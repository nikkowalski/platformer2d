using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {


    public Controller2D target;
    public Vector3 focusAreaSize;
    public float verticalOffset;
    public float cameraDistanceZ = -5;
    public float lookAheadDistanceX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirectionX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    FocusArea focusArea;

    void Start()
    {
        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);


    }


    void LateUpdate()
    {
        focusArea.Update(target.collider.bounds);
        Vector3 focusPosition = focusArea.center + Vector3.up * verticalOffset;

        if(focusArea.velocity.x != 0)
        {
            lookAheadDirectionX = Mathf.Sign(focusArea.velocity.x);

            if(Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStopped = false;
                 targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;


            }
            else
            {
                if(!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX) / 4f;



                }


            }
        }


        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

        focusPosition += Vector3.right * currentLookAheadX;

        transform.position = (Vector3)focusPosition + Vector3.forward * cameraDistanceZ;
    }


    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.25f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);


    }



    struct FocusArea
    {
        public Vector3 center;
        public Vector3 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector3 size)
        {
           

            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector3.zero;
            center = new Vector3((left+right)/2, (top+bottom)/2);

        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if(targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;


            }
            else if(targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;


            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if(targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;


            }else if(targetBounds.max.y > top)
            {

                shiftY = targetBounds.max.y - top;

            }
            top += shiftY;
            bottom += shiftY;
            center = new Vector3((left + right) / 2, (top + bottom) / 2, 0);
            velocity = new Vector3(shiftX, shiftY);

        }


    }


}
