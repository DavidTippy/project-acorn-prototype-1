//This script is based on the series of tutorial videos "Creating a 2D Platformer" by Sebastian Lague on YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//make sure that the player has a BoxCollider2D component
[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour
{

    //create collider and raycastOrigins struct (collider corners)
    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

    public CollisionInfo collisions;

    //distance to inset the rays by
    const float skinWidth = .015f;

    //number of rays
    public int horizontalRayCount = 8;
    public int verticalRayCount = 4;

    //spacing of the rays
    float horizontalRaySpacing;
    float verticalRaySpacing;

    //layer to collide with
    public LayerMask collisionMask;

    // Start is called before the first frame update
    void Start()
    {
        //get the collider
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector2 velocity)
    {
        //make sure we know where the corners of the collider are
        UpdateRaycastOrigins();

        //reset the collision info
        collisions.Reset();

        //figure out if we're going to run into something to our left or right
        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }

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
        float directionX = Mathf.Sign(velocity.x);

        //determine how long the ray needs to be so it can "see" everything we could possibly hit
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

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
    }

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
    }

    

    void UpdateRaycastOrigins()
    {
        //get bounds of collider
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        //get positions of raycast origins (collider corners)
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        //get bounds of collider
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        //clamp number of rays to at least two
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //calculate spacing of rays
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }


    //struct that holds the corners of the collider
    struct RaycastOrigins {

        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
  
    
    }

    public struct CollisionInfo {

        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    
    }



}
