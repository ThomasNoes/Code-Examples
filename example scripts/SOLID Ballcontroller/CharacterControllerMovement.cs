using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CharacterController))]
public class CharacterControllerMovement : MonoBehaviour, IControllable, IMovable, IJumpable
{
    [SerializeField]
    private CharacterController cc;

    private float speed = 5;

    private float jumpForce = 10;
    private float highJumpForce = 200;
    private float longJumpForce = 200;
    private float gravity = -20;

    private Vector3 jumpVector = Vector3.zero;

    private Vector3 moveVector = Vector3.zero;

    void Update()
    {
        //Unity's character controller does not include gravity like rigid bodies
        //Therefore it is included in its own update loop
        //The update loop also ensure the character controller's Move() method is only called once per frame.
        if (!cc.isGrounded)
        {
            jumpVector.y += gravity*Time.deltaTime;
            cc.Move((moveVector + jumpVector)*Time.deltaTime);
        }
        else
        {
            cc.Move(moveVector*Time.deltaTime);
        }
        SetMoveVector(Vector3.zero);

    }

    public void HighJump()
    {
        if (cc.isGrounded)
        {
            SetJumpvector(Vector3.up * highJumpForce);
            AddToMoveVector(jumpVector);
        }
    }

    public void Jump()
    {
        if (cc.isGrounded)
        {
            SetJumpvector(Vector3.up * jumpForce);
            AddToMoveVector(jumpVector);
        }
    }

    public void LongJump()
    {
        if (cc.isGrounded)
        {
            SetJumpvector((Vector3.up + Vector3.forward) * longJumpForce);
            AddToMoveVector(jumpVector);
        }
    }

    public void MoveForward(float input)
    {
        AddToMoveVector(speed * input * Vector3.forward);
    }

    public void MoveRight(float input)
    {
        AddToMoveVector(speed * input * Vector3.right);
    }

    private void SetJumpvector(Vector3 jumpVector)
    {
        this.jumpVector = jumpVector;
    }
    private void SetMoveVector(Vector3 moveVector)
    {
        this.moveVector = moveVector;
    }
    private void AddToMoveVector(Vector3 moveVector)
    {
        this.moveVector += moveVector;
    }
}
