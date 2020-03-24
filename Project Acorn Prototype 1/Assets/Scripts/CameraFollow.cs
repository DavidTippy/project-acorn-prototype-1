//This script is based on the series of tutorial videos "Creating a 2D Platformer" by Sebastian Lague on YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraFollow : MonoBehaviour
{

    Controller2D target;
    public BoxCollider2D cameraCollider;
    public float verticalOffset;
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    FocusArea focusArea;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    bool blockedRight, blockedLeft, blockedTop, blockedBottom;

    void Start()
    {
        //get the camera'scollider
        cameraCollider = GetComponent<BoxCollider2D>();
    }

    void LateUpdate()
    {

        //if we have a player for the camera to target
        if (target ?? false)
        {

            //update the focus area's position.
            focusArea.Update(target.collider.bounds);

            //set the position for the camera to center on
            Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

            //look ahead
            if (focusArea.velocity.x != 0)
            {
                lookAheadDirX = Mathf.Sign(focusArea.velocity.x);

                if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
                {
                    lookAheadStopped = false;
                    targetLookAheadX = lookAheadDirX * lookAheadDstX;
                }
                else
                {
                    if (!lookAheadStopped)
                    {
                        lookAheadStopped = true;
                        targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                    }

                }

            }//end if
            targetLookAheadX = lookAheadDirX * lookAheadDstX;
            currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

            //this line would smooth the Y axis movement - we don't want that for now.
            //focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

            //change the focus position based on the current look ahead
            focusPosition += Vector2.right * currentLookAheadX;

            //Check that the focus position is not out of bounds and move it back inside bounds if it is.
            if (blockedRight && focusPosition.x > transform.position.x)
            {
                focusPosition.x = transform.position.x;
            }
            if (blockedLeft && focusPosition.x < transform.position.x)
            {
                focusPosition.x = transform.position.x;
            }
            if (blockedTop && focusPosition.y > transform.position.y)
            {
                focusPosition.y = transform.position.y;
            }
            if (blockedBottom && focusPosition.y < transform.position.y)
            {
                focusPosition.y = transform.position.y;
            }

            float yValue = 0;
            float xValue = 0;
            Vector3 temp;

            //move the camera, and make sure the camera is in front of the level (on the z-axis)
            
            /*
            if (focusPosition.y > transform.position.y)
            {
                yValue = transform.position.y + 0.1f;
            } else if (focusPosition.y < transform.position.y)
            {
                yValue = transform.position.y - 0.1f;
            } else
            {
                yValue = transform.position.y;
            }

            if (focusPosition.x > transform.position.x)
            {
                xValue = transform.position.x + 0.1f;
            }
            else if (focusPosition.x < transform.position.x)
            {
                xValue = transform.position.x - 0.1f;
            }else
            {
                xValue = transform.position.x;
            }

            temp = new Vector3(xValue, yValue, -10);

    */

            transform.position = Vector3.MoveTowards((Vector3)transform.position,((Vector3)focusPosition + Vector3.forward * -10), 1);

        }
        else // if we do not have a player for the camera to target
        {

            //find the player
            target = GameObject.FindWithTag("Player").GetComponent<Controller2D>();

            focusArea = new FocusArea(target.collider.bounds, focusAreaSize);

        }//end if/else
    }

    //Draw focus area
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }


    //the focus area is a small area around the player where we can move without the camera moving.
    struct FocusArea
    {

        public Vector2 center;
        public Vector2 velocity;

        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = Vector2.zero;
        }

        public void Update(Bounds targetBounds)
        {

            //update left and right bounds
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;


            //update top and bottom bounds
            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            bottom += shiftY;
            top += shiftY;

            //update center
            center = new Vector2((left + right) / 2, (top + bottom) / 2);

            velocity = new Vector2(shiftX, shiftY);

        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("Contact");

        if (other.CompareTag("RightBounds"))
        {
            blockedRight = true;

        } 
        
        if (other.CompareTag("LeftBounds"))
        {
            blockedLeft = true;

        } 

        if (other.CompareTag("TopBounds"))
        {
            blockedTop = true;

        } 

        if (other.CompareTag("BottomBounds"))
        {
            blockedBottom = true;

        }

        if (other.CompareTag("Player"))
        {
        
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("RightBounds"))
        {
            blockedRight = false;

        }

        if (other.CompareTag("LeftBounds"))
        {
            blockedLeft = false;

        }

        if (other.CompareTag("TopBounds"))
        {
            blockedTop = false;

        }

        if (other.CompareTag("BottomBounds"))
        {
            blockedBottom = false;

        }
    }


}
