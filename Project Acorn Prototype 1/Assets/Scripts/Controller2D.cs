//This script is based on the series of tutorial videos "Creating a 2D Platformer" by Sebastian Lague on YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour
{

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

    const float skinWidth = .015f;

    //number of rays
    public int horizontalRayCount = 8;
    public int verticalRayCount = 4;

    //spacing of the rays
    float horizontalRaySpacing;
    float verticalRaySpacing;

    // Start is called before the first frame update
    void Start()
    {
        //get the collider
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector2 velocity)
    {

        UpdateRaycastOrigins();


        //This line moves the player; everything before this line in this method is to figure 
        //out what the velocity should actually be.
        transform.Translate(velocity);
    }

    void VerticalCollisions()
    {

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


    struct RaycastOrigins {

        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    
    
    }


    // Update is called once per frame
    void Update()
    {

        
        

        
        for (int i = 0; i < verticalRayCount; i++)
        {

            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * -2, Color.red);


        }


    }
}
