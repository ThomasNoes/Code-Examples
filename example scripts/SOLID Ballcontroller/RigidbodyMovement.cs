using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class RigidbodyMovement : MonoBehaviour, IControllable, IMovable, IJumpable
{
    [SerializeField]
    private Rigidbody rb;
    
    private float speed = 100;

    private float jumpForce = 1000;
    private float highJumpForce = 2000;
    private float longJumpForce = 2000;

    private bool canJump = true;

    private void AddForceInDirection(float force, Vector3 direction, ForceMode fm = ForceMode.Force)
    {
        if (rb != null)
        {
            direction = direction.normalized;
            rb.AddForce(direction * force * Time.deltaTime, fm);
        }
        else
        {
            Debug.Log(name + " is missing a rigidbody");
        }
    }


    public void HighJump()
    {
        if (GetCanJump())
        {
            AddForceInDirection(highJumpForce, Vector3.up, ForceMode.Impulse);
            SetCanJump(false);
        }
    }

    public void Jump()
    {
        if (GetCanJump())
        {
            AddForceInDirection(jumpForce, Vector3.up, ForceMode.Impulse);
            SetCanJump(false);
        }
    }

    public void LongJump()
    {
        if (GetCanJump())
        {
            AddForceInDirection(jumpForce, Vector3.up + transform.forward, ForceMode.Impulse);
            SetCanJump(false);
        }
    }

    public void MoveForward(float input)
    {
        AddForceInDirection(speed * input, Vector3.forward, ForceMode.VelocityChange);
    }


    public void MoveRight(float input)
    {
        AddForceInDirection(speed * input, Vector3.right, ForceMode.VelocityChange);
    }

    private bool GetCanJump()
    {
        return canJump;
    }

    private void SetCanJump(bool b)
    {
        canJump = b;
    }

    private void OnCollisionEnter(Collision collision)
    {
        SetCanJump(true);
    }

}
