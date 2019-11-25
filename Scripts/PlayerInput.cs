using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    Player player;



	// Use this for initialization
	void Start () {
        player = GetComponent<Player>();

	}
	
	// Update is called once per frame
	void Update () {
        Vector3 directionalInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if ((Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space)))
        {
            player.OnJumpInputDown();


        }

        if (!player.grounded && player.canDoubleJump)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Space))
            {
                player.OnJumpInputDown(); print("DoubleJump performed! ");
                player.canDoubleJump = false; print("player.canDoubleJump: " + player.canDoubleJump);
            }

        }

        


        if ((Input.GetKeyUp(KeyCode.Joystick1Button0) || Input.GetKeyUp(KeyCode.Space)))
        {

            player.OnJumpInputUp();

        }

        


    }


}
