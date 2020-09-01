using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;

    public bool frontWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
    public bool turning; // does this wheel turn stationary car?
    public bool oppositeSteering; // does this wheel apply opposite steer angle?
    public bool brake; // does this wheel break?
}


public class DriveController : MonoBehaviour
{


    [SerializeField] private List<AxleInfo> axleInfos; // the information about each individual axle
    [SerializeField] private float maxMotorTorque; // maximum torque the motor can apply to wheel
    [SerializeField] private float brakeTorque;
    [SerializeField] private float brakeDrag;
    [SerializeField] private float freezeDrag;
    [SerializeField] private float maxReverseTorque;
    [SerializeField] private float maxTurnTorque;
    [SerializeField] private float aerialSteerTorque;
    [SerializeField] private float jumpForce;
    [SerializeField] private float maxSteeringAngle; // maximum steer angle the wheel can have
    [SerializeField] private float minVelocityThreshold;

    float tempDrag;

    float initialSideStiffness;
    float turningSideStiffness = 0.1f;

    Rigidbody rb;
    InputManager2 im;

    void OnEnable()
    {
        ConfigureSubSteps(5, 12, 15);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        im = GetComponent<InputManager2>();
        rb.maxAngularVelocity = 1;
        initialSideStiffness = axleInfos[0].leftWheel.sidewaysFriction.stiffness;

    }

    public void FixedUpdate()
    {
        //ensure the car stays on the ground
        if (CheckWheelsGrounded())
        {
            rb.AddForce(-rb.velocity.magnitude / 10 * transform.up, ForceMode.Acceleration);
        }

        //stop the car velocity if it is low
        if (rb.velocity.magnitude < minVelocityThreshold)
        {
            StopCarForce();
            im.SetIsDriving(false);
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                //axleInfo.leftWheel.steerAngle = steering;
                //axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.oppositeSteering)
            {
                //axleInfo.leftWheel.steerAngle = -steering;
                //axleInfo.rightWheel.steerAngle = -steering;
            }
            if (axleInfo.motor)
            {
                //axleInfo.leftWheel. motorTorque = motor;
                //axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
    // finds the corresponding visual wheel
    // correctly applies the transform
    private void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }



    //functions to make
    public void Accelerate(float input)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = maxMotorTorque * input;
                axleInfo.rightWheel.motorTorque = maxMotorTorque * input;
            }
        }
    }
    public void Brake(float brake)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.brake)
            {
                axleInfo.leftWheel.brakeTorque = brakeTorque * brake;
                axleInfo.rightWheel.brakeTorque = brakeTorque * brake;
                rb.drag = brakeDrag * brake;
            }
        }
    }

    public void Reverse(float input)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = -maxReverseTorque * input;
                axleInfo.rightWheel.motorTorque = -maxReverseTorque * input;
            }
        }
    }
    public void Steer(float horizontal)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = maxSteeringAngle * horizontal;
                axleInfo.rightWheel.steerAngle = maxSteeringAngle * horizontal;
            }
        }
    }
    public void Turn(float horizontal)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.turning)
            {
                SetTurningStiffness(axleInfo, turningSideStiffness);
                axleInfo.leftWheel.motorTorque = maxTurnTorque * horizontal;
                axleInfo.rightWheel.motorTorque = -maxTurnTorque * horizontal;
            }
        }
    }

    public void Jump()
    {
        rb.AddForce(jumpForce * transform.up, ForceMode.VelocityChange);   
    }

    public void AerialSteer(float horizontal, float vertical)
    {
        horizontal *= aerialSteerTorque;
        vertical *= aerialSteerTorque;
        rb.AddRelativeTorque(vertical, horizontal, 0, ForceMode.Acceleration);
    }

    public void Rocket()
    {
        rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
    }

    public void SetTurningStiffness(AxleInfo axleInfo, float newStiffness)
    {
        if (axleInfo.turning)
        {
            ModifySidewaysStiffness(axleInfo.leftWheel, newStiffness);
            ModifySidewaysStiffness(axleInfo.rightWheel, newStiffness);
        }
    }
    public void ResetTurningStiffness()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.turning)
            {
                ModifySidewaysStiffness(axleInfo.leftWheel, initialSideStiffness);
                ModifySidewaysStiffness(axleInfo.rightWheel, initialSideStiffness);
            }
        }
    }

    public void ResetRotation()
    {
        transform.position += Vector3.up;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void ModifySidewaysStiffness(WheelCollider wheel, float newStiffness)
    {

        WheelFrictionCurve sidewaysCurve = wheel.sidewaysFriction; // get the sideways friction curve of the wheel and mofidy that instead
        sidewaysCurve.stiffness = newStiffness;
        wheel.sidewaysFriction = sidewaysCurve;


    }

    private void ConfigureSubSteps(float speedThreshold, int stepsBelowThreshold, int stepsAboveThreshold)
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.leftWheel.ConfigureVehicleSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
            axleInfo.rightWheel.ConfigureVehicleSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
        }
    }

    public void FreezeCar(bool frozen)
    {
        if (frozen)
        {
            im.enabled = false;
            tempDrag = brakeDrag;
            brakeDrag = freezeDrag;
            Brake(1);
        }
        else
        {
            Brake(0);
            brakeDrag = tempDrag;
            im.enabled = true;

        }
    }

    public void StopCarTorque()
    {
        rb.angularVelocity = Vector3.zero;
    }

    public void StopCarForce()
    {
        rb.velocity = Vector3.zero;
    }

    
    //states
    public bool CheckWheelsGrounded()
    {
        int count = 0;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.rightWheel.isGrounded)
            {
                count++;
            }
            if (axleInfo.leftWheel.isGrounded)
            {
                count++;
            }
        }
        if (count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

