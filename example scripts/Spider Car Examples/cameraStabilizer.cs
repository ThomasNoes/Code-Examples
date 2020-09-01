using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraStabilizer : MonoBehaviour
{

    public GameObject carPrefab;
    Quaternion newQuat = Quaternion.identity;

    InputManager2 im;

    float rotX;
    float rotY;
    float rotZ;
    float lerpDamping = 1;

    ViewPoint targetView;

    // Start is called before the first frame update
    void Start()
    {
        im = GetComponentInParent<InputManager2>();
        
        LocalResetToWorldSpace();
    }

    // Update is called once per frame
    void Update()
    {
        ResetToWorldSpace();
        FollowRotationOf(carPrefab);
    }
    void LocalResetToWorldSpace()
    {
        //make sure rotation is reset to wourld space
        transform.localRotation = Quaternion.EulerAngles(Vector3.zero);
    }
    void ResetToWorldSpace()
    {
        //make sure rotation is reset to wourld space
        transform.rotation = Quaternion.EulerAngles(Vector3.zero);
    }



    void FollowRotationOf(GameObject obj)
    {
        if(obj)
        {
            Vector3 objRot = Vector3.zero;

            if (targetView.GetRotateAlongX() || im.IsGrounded)  //also follow x rotation of the car is on the ground
            {
                rotX = obj.transform.rotation.eulerAngles.x;
            }
            else                                                // if not on ground lerp the x rotation back to 0/360 for easy view
            {
                if(rotX <180)                   
                {
                    rotX = LerpFloatToX(rotX, 0);
                }
                else
                {
                    rotX = LerpFloatToX(rotX, 360);
                }
            }
            if (targetView.GetRotateAlongY())
            {
                rotY = obj.transform.rotation.eulerAngles.y;
            }
            else
            {
                if (rotY < 180)
                {
                    rotY = LerpFloatToX(rotY, 0);
                }
                else
                {
                    rotY = LerpFloatToX(rotY, 360);
                }
            }
            if (targetView.GetRotateAlongZ())
            {
                rotZ = obj.transform.rotation.eulerAngles.z;
            }
            else
            {
                if (rotZ < 180)
                {
                    rotZ = LerpFloatToX(rotZ, 0);
                }
                else
                {
                    rotZ = LerpFloatToX(rotZ, 360);
                }
            }
            objRot = new Vector3(rotX, rotY, rotZ);  
            
        
            newQuat.eulerAngles = objRot;
            transform.rotation = newQuat;
        }
    }

    float LerpFloatToX(float f, float x)
    {
        return Mathf.Lerp(f, x, Time.deltaTime * lerpDamping);
    }

    public void SetTargetView(ViewPoint targetView)
    {
        this.targetView = targetView;
    }
}
