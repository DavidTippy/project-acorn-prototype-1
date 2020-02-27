using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour
{

    //layer to collide with
    public LayerMask collisionMask;

    //distance to inset the rays by
    public const float skinWidth = .015f;

    //number of rays
    public int horizontalRayCount = 8;
    public int verticalRayCount = 4;

    //spacing of the rays
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    //create collider and raycastOrigins struct (collider corners)
    [HideInInspector]
    public BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;
    public virtual void Start()
    {
        //get the collider
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
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

    public void CalculateRaySpacing()
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
    public struct RaycastOrigins
    {

        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;


    }
}
