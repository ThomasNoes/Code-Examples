using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    //This script is for handling the input for moving a character around  
    //It translates the input into actions for any object that implements the IControllable interface
    //The character does not need to be a ball but for that is just what I chose for this example, hence the name BallController
   
    private IControllable character;


    private void Awake()
    {
        character = GetComponent<IControllable>();

    }

    // Update is called once per frame
    void Update()
    {
        if (character is IMovable) // if the character implements the Imovable interface it can move using the axes called Vertical and Horizontal
        {
            IMovable moveable = character as IMovable;
            moveable.MoveForward(Input.GetAxis("Vertical"));
            moveable.MoveRight(Input.GetAxis("Horizontal"));
        }
        if (character is IJumpable) // if the character implements the Ijumpable interface, it can jump
        {
            IJumpable jumpable = character as IJumpable;

            if (Input.GetButtonDown("Jump"))
            {
                jumpable.Jump();
            }
        }
    }
}


