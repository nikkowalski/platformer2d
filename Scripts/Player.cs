using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    Controller2D controller;


    public Vector3       wallJumpOff;
    public Vector3       wallJumpClimb;
    public Vector3       wallLeap;

    [Range(0.01f, 3.0f)]
    public float slidingSlopeSpeed;

    Vector3 directionalInput;


    public float        maxJumpHeight              =   10.00f;
    public float        minJumpHeight              =   3.00f;
    public float        timeToJumpApex             =   0.40f;
    public float        wallSlideSpeedMax          =   3.00f;
    public float        wallStickTime              =   0.25f;
           float        timeToWallUnstick          =   0.00f;
           float        gravity                    = -20.00f;
           float        moveSpeed                  =   6.00f;
           float        maxJumpVelocity            =   18.00f;
           float        minJumpVelocity;
           float        accelerationTimeAirborne   =   0.20f;
           float        accelerationTimeGrounded   =   0.10f;
           float        velocityXSmoothing;

    bool leftTrigger  = false;
    bool rightTrigger = false;
    bool leftD_Pad    = false;
    bool rightD_Pad   = false;
    bool upD_Pad      = false;
    bool downD_Pad    = false;
    bool wallSliding;


    public bool grounded;
    public bool canDoubleJump = false;

    public int notes = 0;

    int wallDirX;


    Vector3 velocity;










    // Use this for initialization
    void Start () {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + " , Jump Velocity: " + maxJumpVelocity);

	}


    // Update is called once per frame
    void Update()
    {
        CheckIfGrounded();
        CalculateVelocity();
        HandleWallSliding();
        HandleJoystickExtraAxis();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;


            }
        }

    }


    public void SetDirectionalInput(Vector3 input)
    {

        directionalInput = input;

    }

    public void OnJumpInputDown()
    {

        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;

            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;

            }
        }

        if (controller.collisions.below)
        {
            

            if (controller.collisions.slidingDownMaxSlope)
            {
                if(directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) // not jumping against max slope
                {
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;


                }

            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
            
        }
        if(canDoubleJump)
        {
            velocity.y = maxJumpVelocity;

        }
        

        


    }

    public void OnJumpInputUp()
    {

        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;


        }
        if (canDoubleJump)
        {
            velocity.y = minJumpVelocity;

        }

    }

    void CheckIfGrounded()
    {
        if (controller.collisions.below)
        {
            grounded = true; print("grounded: " + grounded);
            canDoubleJump = true; print("canDoubleJump: " + canDoubleJump);
        }
        else
        {
            grounded = false;
           
            
        }



    }

    void HandleWallSliding()
    {

        wallDirX = (controller.collisions.left) ? -1 : 1;

        wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;
           


            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;

            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;
                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;

                }
            }
            else
            {

                timeToWallUnstick = wallStickTime;

            }
        }



    }

    void CalculateVelocity()
    {

        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;



    }

    void HandleJoystickExtraAxis()
    {

        if (Input.GetAxisRaw("Axis3") == -1)
        {

            leftTrigger = true;

        }
        else
        {
            leftTrigger = false;
        }

        if (Input.GetAxisRaw("Axis3") == 1)
        {

            rightTrigger = true;

        }
        else
        {
            rightTrigger = false;
        }
        


        if (Input.GetAxisRaw("Axis6") == -1)
        {

            leftD_Pad = true;

        }
        else
        {
            leftD_Pad = false;
        }

        if (Input.GetAxisRaw("Axis6") == 1)
        {

            rightD_Pad = true;

        }
        else
        {
            rightD_Pad = false;
        }
               

        if (Input.GetAxisRaw("Axis7") == -1)
        {

            downD_Pad = true;

        }
        else
        {
            downD_Pad = false;
        }

        if (Input.GetAxisRaw("Axis7") == 1)
        {

            upD_Pad = true;

        }
        else
        {
            upD_Pad = false;
        }

    }


}
