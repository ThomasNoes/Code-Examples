using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Position of camerapoint in different views")]
    public Transform defTrans;
    public float defDis;
    public Transform farTrans;
    public float farDis;
    public Transform aimTrans;
    public float aimDis;
    public Transform backTrans;
    public float backDis;

    ViewPoint defView;
    ViewPoint farView;
    ViewPoint aimView;
    ViewPoint backView;
    ViewPoint targetView;

    [Header("Needed gameobjects for the script to work")]
    public Transform point;
    public Transform pivot;
    public Transform player;

    [Space(10)]

    public float translateSpeed;
    public float translateDamping;

    public float orbitSpeed;
    public float orbitDamping; 

    public float zoomSpeed;
    public float zoomDamping;

    float maxRotation = 60, minRotation = -60;
    public float currentCameraDistance;
    Transform targetTransform;
    Vector3 localRotation;

    bool aimMode =false;
    bool farMode = false;
    bool backMode = false;

    Quaternion startQuat;

    //raycasting
    int layerMask;
    RaycastHit hit;


    cameraStabilizer cs;


    // Start is called before the first frame update
    void Start()
    {
        cs = GetComponentInParent<cameraStabilizer>();
        //make the views
        UpdateViewPoint();

        GoToDefView();

        targetTransform = transform;

        // raycasting
        layerMask = ~LayerMask.GetMask("Car"); //set layerMask to exclude Car layer

        startQuat = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
        UpdateViewPoint(); //added for easy debugging/testing of views

        RayCheckObscurity();
        
        //actual transformation of camera point
        pivot.localPosition = Vector3.Lerp(pivot.localPosition, targetView.GetLocalPosition(), Time.deltaTime * translateDamping);
        
        //actual rotation of camera
        pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(localRotation.x, localRotation.y, 0), Time.deltaTime * orbitDamping);

        //actual zoom of the camera
        if (transform.localPosition.z != currentCameraDistance * -1)
        {
            transform.localPosition = new Vector3(
                0f,
                0f,
                Mathf.Lerp(transform.localPosition.z, currentCameraDistance * -1f, Time.deltaTime * zoomDamping)
            );
        }
    }

    void SwingCamera()
    {
        //targetTransform.up = Vector3.up;
        localRotation.x = maxRotation;
    }

    public void RotateCamera(float vertical, float horizontal)
    {
        localRotation.x += InputSpeed(vertical);
        localRotation.y += InputSpeed(horizontal);

        //clamping on y to avoid the camera flipping 

        localRotation.x = Mathf.Clamp(localRotation.x,minRotation,maxRotation);


    }

    void ActualRotation()
    {
        pivot.rotation = Quaternion.Euler(localRotation.x, localRotation.y, 0) * player.rotation;
        pivot.rotation = Quaternion.Lerp(pivot.rotation, targetTransform.rotation, Time.deltaTime * orbitDamping);
    }

    float InputSpeed(float input)
    {
        return input * Time.deltaTime * orbitSpeed;
    }

    void lookAtPosition(Vector3 pos)
    {
        transform.LookAt(pos, pivot.transform.up);
    }

    public void SwitchCameraMode(bool farMode)
    {
        this.farMode = farMode;
        SwitchViewMode();
    }

    void SwitchViewMode()
    {
        if(aimMode)
        {

        }
        else if (backMode)
        {
            
        }
        else if (farMode)
        {
        }
        else
        {
        }
        ResetCameraRotation();
    }

    public void AddCameraRotationToPlayer()
    {
        player.rotation = point.rotation * Quaternion.Euler(localRotation);
        
        //ResetCameraRotation();
    }

    public void GoToFarView()
    {
        if(farMode)
        {
            GoToDefView();
        }
        else
        {
            farMode = true;
            aimMode = false;
            backMode = false;
            SwitchViewPoint(farView);
        }
    }
    public void GoToAimView()
    {
        if (aimMode)
        {
            GoToDefView();
        }
        else
        {
            farMode = false;
            aimMode = true;
            backMode = false;
            SwitchViewPoint(aimView);
        }
    }
    public void GoToBackView()
    {
        if (backMode)
        {
            GoToDefView();
        }
        else
        {
            farMode = false;
            aimMode = false;
            backMode = true;
            SwitchViewPoint(backView);
        }
    }
    public void GoToDefView()
    {
        ResetCameraModes();
        SwitchViewPoint(defView);

    }
    private void ResetCameraModes()
    {
        farMode = false;
        aimMode = false;
        backMode = false;
    }

    void SwitchViewPoint(ViewPoint target)
    {
        Debug.Log("New View is " + target.GetName());
        targetView = target;
        currentCameraDistance = targetView.GetCameraDistance();
        cs.SetTargetView(target);
    }

    public void ResetCameraRotation()
    {
        localRotation = Vector3.zero;
    }

    public ViewPoint getTargetView()
    {
        return targetView;
    }

    void UpdateViewPoint()
    {
        defView = new ViewPoint("Default View", defTrans, defDis, false, true, false);
        farView = new ViewPoint("Far View", farTrans, farDis, false, true, false);
        aimView = new ViewPoint("Aim View", aimTrans, aimDis, false, true, false);
        backView = new ViewPoint("Back View", backTrans, backDis, false, true, false);
    }

    void RayCheckObscurity()
    {
        Vector3 carToCam = transform.position - point.position;

        if (Physics.Raycast(point.position, carToCam, out hit, targetView.GetCameraDistance(),layerMask))
        {
            SetCurrentCameraDistance(CalculateObscuringDistance());
            lookAtPosition(point.position);
        }
        else
        {
            SetCurrentCameraDistance(targetView.GetCameraDistance());
            transform.localRotation = startQuat;
        } 
    }
    void SetCurrentCameraDistance(float newDistance)
    {
        currentCameraDistance = newDistance;
    }

    float CalculateObscuringDistance()
    {
        return (hit.point - point.position).magnitude;
    }


}
