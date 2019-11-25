using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        

    }

    private void OnCollisionEnter(Collision collision)
    {
        //check if you collided with the player
        //play sound
        //move to hidden locztion at -100x, -100y, -100z
    }

}
