﻿//This script is based on the series of tutorial videos "Creating a 2D Platformer" by Sebastian Lague on YouTube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour
{

    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        //get jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {

            player.OnJumpInputDown();
            player.canJumpEarly = (player.velocity.y < 0 && !player.wallSliding) ? true : false;
            Debug.Log(player.canJumpEarly);


        }//end if

        if (Input.GetKeyUp(KeyCode.Space))
        {

            player.OnJumpInputUp();
            player.canJumpEarly = false;

        }//end if

        if (Input.GetKey(KeyCode.Space))
        {

            player.OnJumpInputHeld();

        }//end if


    }
}
