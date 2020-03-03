//This script is based on the series of tutorial videos "Creating a 2D Platformer" by Sebastian Lague on YouTube

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour
{

    //variable to allow player to jump by pressing the jump button while 
    //traveling down and holding it until they hit the ground
    public bool canJumpEarly;

    //variables to alllow the player to jump for a few frames after walking off a ledge
    public bool canJumpLate;
    public float lateJumpTime = .5f;
    public float lateJumpCounter;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1.5f;
    public float timeToJumpApex = .3f;

    float accelerationTimeAirborne = .3f;
    float accelerationTimeGrounded = .1f;

    public float moveSpeed = 7;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;

    [HideInInspector]
    public Vector2 velocity;
    public Vector2 wallJumpForce;

    public float wallSlideSpeedMax = 5;

    public float wallStickTime = 0.01f;
    public float timeToWallUnstick = 0.01f;

    float velocityXSmoothing;

    //create Controller2D script
    Controller2D controller;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;



    // Start is called before the first frame update
    void Start()
    {
        //get Controller2D attached to player object
        controller = GetComponent<Controller2D>();

        //calculate the gravity from the jumpHeight and timeToJumpApex
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        //calculate the maxjumpVelocity and minJumpVelocity
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

    }//end Start method



    // Update is called once per frame
    void Update()
    {
        HandleLateJumps();
        CalculateVelocity();
        HandleWallSliding();

        //call controller's move method. this method will calculate how much to move the player to avoid collisions
        controller.Move(velocity * Time.deltaTime);

        //reset player's vertical momentum if they hit a floor or ceiling
        if (controller.collisions.above || controller.collisions.below)
        {           
            velocity.y = 0;
        }//end if

    }//end Update method

    

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    
    
    public void OnJumpInputDown()
    {

        if (wallSliding)
        {
            //only allow wall jumps if the input is in the opposite direction from the wall
            if ((directionalInput.x != wallDirX) && (directionalInput.x != 0))
            {
                velocity.x = -wallDirX * wallJumpForce.x;
                velocity.y = wallJumpForce.y;
            }


        }//end if

        //allow a regular jump if we're standing on the ground
        if (controller.collisions.below)
        {

            velocity.y = maxJumpVelocity;

        }//end if


    }



    public void OnJumpInputUp()
    {
        //set the y velocity to minJumpVelocity if it is greater than minJumpVelocity
        velocity.y = (velocity.y > minJumpVelocity) ? minJumpVelocity : velocity.y;
    }



    public void OnJumpInputHeld()
    {
        //allow a regular jump if we're standing on the ground
        if ((controller.collisions.below && canJumpEarly ) || canJumpLate)
        {

            velocity.y = maxJumpVelocity;
            canJumpEarly = false;
            canJumpLate = false;

        }//end if
    }



    private void HandleLateJumps()
    {
        
        //give the player a short window in which to input a jump after they've walked off a block.
        if(controller.collisions.below)
        {
            canJumpLate = true;
            lateJumpCounter = lateJumpTime;
        } else if(lateJumpCounter > 0)
        {
            lateJumpCounter -= Time.deltaTime;
        } else
        {
            canJumpLate = false;
        }

        //don't allow a late jump if we're moving upwards
        if(velocity.y > 0)
        {
            canJumpLate = false;
        }
        
    } 



    void HandleWallSliding()
    {
        //get the direction of the wall we're colliding with
        wallDirX = ((controller.collisions.left) ? -1 : 1);

        wallSliding = false;

        //check if player is wall sliding
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {

            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;

            }//end if

            //This block causes us to slide off the wall after a certain length of time holding the key 
            //away from the wall, thereby making the wall jump more difficult to perform
            //(since wallStickTime and timeToWallUnstick are public, they have to be changed in the inspector)
            if ((directionalInput.x != wallDirX && directionalInput.x != 0) && (timeToWallUnstick > 0))
            {

                timeToWallUnstick -= Time.deltaTime;
                velocityXSmoothing = 0;
                velocity.x = 0;


            }
            else
            {
                velocity.x = directionalInput.x;
                timeToWallUnstick = wallStickTime;
            }//end if/else

        }//end if
    }



    void CalculateVelocity()
    {
        //apply player's horizontal input smoothly
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        //apply gravity
        velocity.y += gravity * Time.deltaTime;
    }

   
}
