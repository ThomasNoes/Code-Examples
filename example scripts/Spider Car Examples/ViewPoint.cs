using UnityEngine;

public class ViewPoint
{
    private string name;
    private Transform point;
    private float cameraDistance;

    private bool rotateAlongX;
    private bool rotateAlongY;
    private bool rotateAlongZ;


    public ViewPoint(string name, Transform point, float cameraDistance, bool x, bool y, bool z)
    {
        this.name = name;
        this.point = point;
        this.cameraDistance = cameraDistance;

        this.rotateAlongX = x;
        this.rotateAlongY = y;
        this.rotateAlongZ = z;
    }

    public string GetName()
    {
        return name;
    }

    public Vector3 GetLocalPosition()
    {
        return point.localPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return point.position;
    }

    public float GetCameraDistance()
    {
        return cameraDistance;
    }
    public bool GetRotateAlongX()
    {
        return rotateAlongX;
    }
    public bool GetRotateAlongY()
    {
        return rotateAlongY;
    }
    public bool GetRotateAlongZ()
    {
        return rotateAlongZ;
    }
}
