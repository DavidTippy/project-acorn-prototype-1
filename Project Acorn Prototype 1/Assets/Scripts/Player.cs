//This script is based on the series of tutorial videos "Creating a 2D Platformer" by Sebastian Lague on YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour
{

    public float jumpHeight = 4;
    public float timeToJumpApex = .3f;

    float accelerationTimeAirborne = .3f;
    float accelerationTimeGrounded = .1f;

    float moveSpeed = 8;
    float gravity;
    float jumpVelocity;

    Vector2 velocity;

    float velocityXSmoothing;

    //create Controller2D script
    Controller2D controller;

    // Start is called before the first frame update
    void Start()
    {
        //get Controller2D attached to player object
        controller = GetComponent<Controller2D>();

        //calculate the gravity from the jumpHeight and timeToJumpApex
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        //calculate the jumpVelocity
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        //print gravity and jumpVelocity
        //print("Gravity: " + gravity + "Jump Velocity: " + jumpVelocity);
    }

    // Update is called once per frame
    void Update()
    {

        //reset player's vertical momentum if they hit a floor or ceiling
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        //get player's input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //get jump input
        if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {

            velocity.y = jumpVelocity;

        }

        //apply player's horizontal input smoothly
        float targetVelocityX =  input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

        //apply gravity
        velocity.y += gravity * Time.deltaTime;

        //call controller's move method. this method will calculate how much to move the player to avoid collisions
        controller.Move(velocity * Time.deltaTime);
    }
}
