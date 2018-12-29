using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController{

    

    public CollisionInfo        collisions;

    //floats
   public  float                maxSlopeAngle               =   45.0f;
   

    [HideInInspector]
    public Vector3 playerInput;

  public override void Start()
    {
        base.Start();

        collisions.faceDirection = 1;
    }


    public void Move(Vector3 amtToMove, bool standingOnPlatform)
    {

        Move(amtToMove, Vector3.zero, standingOnPlatform);
        

    }


    public void Move(Vector3 amtToMove, Vector3 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        playerInput = input;

        if(amtToMove.y < 0)
        { DecendSlope(ref amtToMove); }

        if (amtToMove.x != 0)
        {
            collisions.faceDirection = (int)Mathf.Sign(amtToMove.x);

        }


        HorizontalCollisions(ref amtToMove); 

        if (amtToMove.y != 0)
        { VerticalCollisions(ref amtToMove); }

        transform.Translate(amtToMove);

        if(standingOnPlatform)
        {

            collisions.below = true;

        }

    }

    void HorizontalCollisions(ref Vector3 amtToMove)
    {
        float directionX = collisions.faceDirection;
        float rayLength = Mathf.Abs(amtToMove.x) + skinWidth;

        if(Mathf.Abs(amtToMove.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;


        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector3.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.right * directionX, rayLength, collisionMask);

            //Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * -2, Color.red);
            Debug.DrawRay(rayOrigin, Vector3.right * directionX, Color.red);

            if (hit)
            {
                if(hit.distance == 0)
                {
                    continue;


                }

                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if ((i == 0) && (slopeAngle <= maxSlopeAngle))
                {
                    float distanceToSlopeStart = 0;
                    if(slopeAngle != collisions.oldSlopeAngle)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        amtToMove.x -= distanceToSlopeStart * directionX;
                    }

                    ClimbSlope(ref amtToMove, slopeAngle, hit.normal);
                    amtToMove.x += distanceToSlopeStart * directionX;

                    //print("Slope Angle: " + slopeAngle);
                }

                if ((!collisions.climbingSlope) || (slopeAngle > maxSlopeAngle))
                {
                    amtToMove.x = (hit.distance - skinWidth) * directionX;

                    rayLength = hit.distance;

                    if(collisions.climbingSlope)
                    {
                        amtToMove.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(amtToMove.x);


                    }


                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }

            }


        }

    }

    void VerticalCollisions(ref Vector3 amtToMove)
    {
        float directionY = Mathf.Sign(amtToMove.y);
        float rayLength = Mathf.Abs(amtToMove.y) + skinWidth;

        for(int i =0; i<verticalRayCount; i++)
        {
            Vector3 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector3.right * (verticalRaySpacing * i + amtToMove.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector3.up * directionY, Color.red);

            if(hit)
            {
                if(hit.collider.tag == "Through")
                {
                    if((directionY == 1)||(hit.distance == 0))
                    {
                        continue;


                    }
                    if(collisions.fallingThroughPlatform)
                    {

                        continue;

                    }


                    if(playerInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", 0.5f);
                        continue;

                    }
                }


                amtToMove.y = (hit.distance - skinWidth) * directionY;

                rayLength = hit.distance;

                if(collisions.climbingSlope)
                {
                    amtToMove.x = amtToMove.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(amtToMove.x);


                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

            }


        }

        if(collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(amtToMove.x);
            rayLength = Mathf.Abs(amtToMove.x) + skinWidth;
            Vector3 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector3.up * amtToMove.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.right * directionX, rayLength, collisionMask);

            if(hit)
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if(slopeAngle != collisions.slopeAngle)
                {
                    amtToMove.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;

                }


            }

        }
        

    }

    void ClimbSlope(ref Vector3 amtToMove, float slopeAngle, Vector3 slopeNormal)
    {

        float moveDistance = Mathf.Abs(amtToMove.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (amtToMove.y <= climbVelocityY)
        {
            amtToMove.y = climbVelocityY;
            amtToMove.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(amtToMove.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }

    }

    void DecendSlope(ref Vector3 amtToMove)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector3.down, Mathf.Abs(amtToMove.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector3.down, Mathf.Abs(amtToMove.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref amtToMove);
            SlideDownMaxSlope(maxSlopeHitRight, ref amtToMove);
        }

        if(!collisions.slidingDownMaxSlope)
        { 
            float directionX = Mathf.Sign(amtToMove.x);
            Vector3 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector3.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if ((slopeAngle != 0) && (slopeAngle <= maxSlopeAngle))
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if ((hit.distance - skinWidth) <= (Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(amtToMove.x)))
                        {

                            float moveDistance = Mathf.Abs(amtToMove.x);
                            float decendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                            amtToMove.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(amtToMove.x);
                            amtToMove.y -= decendVelocityY;
                            collisions.slopeAngle = slopeAngle;
                            collisions.decendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }


                    }


                }


            }

        }

    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector3 amtToMove)
    {
        if(hit)
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if(slopeAngle>maxSlopeAngle)
            {
                amtToMove.x = Mathf.Sign(hit.normal.x) * ((Mathf.Abs(amtToMove.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad));

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }


        }

    }

    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;


    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope, decendingSlope;
        public bool fallingThroughPlatform;
        public bool slidingDownMaxSlope;

        public int faceDirection;

        public Vector3 slopeNormal;

        public float slopeAngle, oldSlopeAngle;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            decendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector3.zero;

            oldSlopeAngle = slopeAngle;
            slopeAngle = 0;
        }


    }

    
}

