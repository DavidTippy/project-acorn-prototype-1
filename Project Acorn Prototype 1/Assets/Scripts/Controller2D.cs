//This script is based on the series of tutorial videos "Creating a 2D Platformer" by Sebastian Lague on YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//make sure that the player has a BoxCollider2D component
public class Controller2D : RaycastController
{

    public CollisionInfo collisions;

    //This method calls the start method in RaycastController 
    //while still allowing us to put things in the start method for Controller2D
    public override void Start()
    {
        base.Start();

        //set the player to face right
        collisions.faceDir = 1;
    }

    public void Move(Vector2 velocity)
    {

        

        //make sure we know where the corners of the collider are
        UpdateRaycastOrigins();

        //reset the collision info
        collisions.Reset();

        //set the direction the player is facing
        if (velocity.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(velocity.x);
        }

        // figure out if we're going to run into something to our left or right
        //if (velocity.x != 0)
        //{
            HorizontalCollisions(ref velocity);
        //}
        

        //figure out if we've hit something above or below us
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        //This line moves the player; everything before this line in this method is to figure 
        //out what the velocity should actually be.
        transform.Translate(velocity);
    }

    void HorizontalCollisions(ref Vector2 velocity)
    {
        //determine if we are traveling left or right
        float directionX = collisions.faceDir;

        //determine how long the ray needs to be so it can "see" everything we could possibly hit
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        //make sure we can hit the wall with a ray even if we are not moving (for wall jump)
        if(Mathf.Abs(velocity.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        //loop throught horizontal rays to see if we hit something
        for (int i = 0; i < horizontalRayCount; i++)
        {

            //use the top rayOrigin if we are moving up, use the bottom rayOrigin if we are moving down
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            //calculate where to put the next ray
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            //check for hit
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            //Draw rays
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                //change velocity to avoid going inside wall
                velocity.x = (hit.distance - skinWidth) * directionX;

                //Change ray length if we hit something so we can't hit something further away 
                rayLength = hit.distance;

                //update collision info
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;

            }//end if

        }//end for
    }//end method HorizontalCollisions



    void VerticalCollisions(ref Vector2 velocity)
    {
        //determine if we are traveling up or down
        float directionY = Mathf.Sign(velocity.y);

        //determine how long the ray needs to be so it can "see" everything we could possibly hit
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        //loop throught vertical rays to see if we hit something
        for (int i = 0; i < verticalRayCount; i++)
        {

            //use the top rayOrigin if we are moving up, use the bottom rayOrigin if we are moving down
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            //calculate where to put the next ray
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            //check for hit
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            //Draw rays
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if(hit)
            {

                //change velocity to avoid going inside wall
                velocity.y = (hit.distance - skinWidth) * directionY;

                //Change ray length if we hit something so we can't hit something further away 
                rayLength = hit.distance;

                //update collision info
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

            }//end if

        }//end for
    }//end method VerticalCollisions
 


    public struct CollisionInfo {

        public bool above, below;
        public bool left, right;

        //direction the player is facing
        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    
    }

}
