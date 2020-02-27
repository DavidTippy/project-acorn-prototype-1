﻿//This script is based on the series of tutorial videos "Creating a 2D Platformer" by Sebastian Lague on YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour
{

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1.5f;
    public float timeToJumpApex = .3f;

    float accelerationTimeAirborne = .3f;
    float accelerationTimeGrounded = .1f;

    public float moveSpeed = 7;
    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;

    Vector2 velocity;
    public Vector2 wallJumpForce;

    public float wallSlideSpeedMax = 5;

    public float wallStickTime = 0.01f;
    public float timeToWallUnstick = 0.01f;

    float velocityXSmoothing;

    //create Controller2D script
    Controller2D controller;

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


        //print gravity and jumpVelocity
        //print("Gravity: " + gravity + "Jump Velocity: " + jumpVelocity);
    }//end Start method

    // Update is called once per frame
    void Update()
    {

        //get player's input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //get the direction of the wall we're colliding with
        int wallDirX = ((controller.collisions.left) ? -1 : 1);

        //apply player's horizontal input smoothly
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        bool wallSliding = false;

        //check if player is wall sliding
        if((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {

            wallSliding = true;

            if(velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;

            }//end if

                //This block causes us to slide off the wall after a certain length of time holding the key 
                //away from the wall, thereby making the wall jump more difficult to perform
                //(since wallStickTime and timeToWallUnstick are public, they have to be changed in the inspector)
                if ((input.x != wallDirX && input.x != 0) && (timeToWallUnstick > 0))
                {
     
                        timeToWallUnstick -= Time.deltaTime;
                        velocityXSmoothing = 0;
                        velocity.x = 0;                   


                } else
                {
                    velocity.x = input.x;
                    timeToWallUnstick = wallStickTime;
                }//end if/else

        }//end if

    

        //reset player's vertical momentum if they hit a floor or ceiling
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }//end if

        //get jump input
        if(Input.GetKeyDown(KeyCode.Space))
        {

            if (wallSliding)
            {
                //only allow wall jumps if the input is in the opposite direction from the wall
                if((input.x != wallDirX) && (input.x != 0))
                {
                    velocity.x = -wallDirX * wallJumpForce.x;
                    velocity.y = wallJumpForce.y;
                }


            }//end if

            //allow a regular jump if we're standing on the ground
            if(controller.collisions.below)
            {
                velocity.y = maxJumpVelocity;
            }//end if
            
        }//end if
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //set the y velocity to minJumpVelocity if it is greater than minJumpVelocity
            velocity.y = (velocity.y > minJumpVelocity) ? minJumpVelocity : velocity.y;

        }//end if

        //apply gravity
        velocity.y += gravity * Time.deltaTime;

        //call controller's move method. this method will calculate how much to move the player to avoid collisions
        controller.Move(velocity * Time.deltaTime);

    }//end Update method
}
