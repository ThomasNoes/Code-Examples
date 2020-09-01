using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardainCalibration : MonoBehaviour
{
    //This script calibrates the object, it is attached to, to the center and orientation of the Oculus Quest guardian system 
    //which is not natively supported by Oculus for Unity

    public bool calibrateGameObject;

    public bool drawPlayArea;

    public GameObject cornerGizmo;

    public GameObject centerGizmo;

    Vector3 centerOffset;

    Vector3[] points;

    Vector3 center;
    Vector3 forward;

    // Start is called before the first frame update
    void Awake()
    {
        if (!Application.isEditor)
        {
            Calibrate(out center, out forward);
            if (calibrateGameObject)
            {
                transform.position = new Vector3(center.x, 0, center.z);;
                transform.forward = center - forward;
            }
        }
    }

    public void DebugCalibration(out Vector3 center, out Vector3 forward)
    {
        float posX = Random.Range(-1.0f, 1.0f);
        float posZ = Random.Range(-1.0f, 1.0f);
        float rotX = Random.Range(-1.0f, 1.0f);
        float rotZ = Random.Range(-1.0f, 1.0f);

        center = new Vector3(posX, transform.position.y, posZ);
        forward = new Vector3(rotX,0, rotZ);
    }

    public void Calibrate(out Vector3 center, out Vector3 forward)
    {
        points = GetBoundaryPoints();
        IgnoreY(points);
        SetReferencePoints(out center, out forward);
        DrawCalibrationArea(center, forward);
    }

    public bool RoomScaling3x4Check(float tileWidth)
    {
        float colLength = 0;
        float rowLength = 0;

        points = GetBoundaryPoints();

        if (Vector3.Distance(points[0], points[1]) > Vector3.Distance(points[1], points[2]))
        {
            colLength = Vector3.Distance(points[0], points[1]);
            rowLength = Vector3.Distance(points[1], points[2]);
        }
        else
        {
            colLength = Vector3.Distance(points[1], points[2]);
            rowLength = Vector3.Distance(points[0], points[1]);
        }

        if(Mathf.FloorToInt(rowLength / tileWidth) >= 3 && Mathf.FloorToInt(colLength / tileWidth) >=4)
        {
            return true;
        }
        return false;
    }

    public void RoomScaling(out int maxRows, out int maxColumns, float tileWidth, float bufferWidth = 0, bool debugging = false)
    {
        float colLength=0;
        float rowLength=0;

        if (debugging)
        {
            //colLength = 4;
            //rowLength = 3;
            //Debug.Log("debug room scale");
            maxRows = 3;
            maxColumns = 4;
            return;
        }
        else
        {
            points = GetBoundaryPoints();

            if (Vector3.Distance(points[0], points[1]) > Vector3.Distance(points[1], points[2]))
            {
                colLength = Vector3.Distance(points[0], points[1]);
                rowLength = Vector3.Distance(points[1], points[2]);
            }
            else
            {
                colLength = Vector3.Distance(points[1], points[2]);
                rowLength = Vector3.Distance(points[0], points[1]);
            }
        }
        
        maxRows = Mathf.FloorToInt((rowLength-2.0f*bufferWidth) / tileWidth);
        maxColumns = Mathf.FloorToInt((colLength-2.0f*bufferWidth) / tileWidth);

    }

    public void DrawCalibrationArea(Vector3 center, Vector3 forward)
    {
        if (drawPlayArea)
        {
            if (cornerGizmo)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    DrawGizmo(points[i], cornerGizmo, 0.1f);
                }
            }
            if (centerGizmo)
            {
                DrawGizmo(center, centerGizmo, 0.1f);
                DrawGizmo(forward, centerGizmo, 0.1f);
            }
        }
    }

    private void DrawGizmo(Vector3 point, GameObject gizmo, float scale = 0.05f)
    {
        GameObject temp = Instantiate(gizmo, new Vector3(0,0,0),Quaternion.identity);
        temp.transform.position = new Vector3(point.x, point.y, point.z);
        temp.transform.localScale = new Vector3(scale, scale, scale);

    }

    public static Vector3[] GetBoundaryPoints()
    {
        return OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
    }

    public static float GetShortestDimension()
    {
        Vector3[] points = GetBoundaryPoints();

        //first Dimension
        float width = Vector3.Distance(points[0], points[1]);
        //second Dimension
        float length = Vector3.Distance(points[1], points[2]);

        if (length <=width)
        {
            return length;
        }
        return width;

    }

    public void ScaleObject()
    {
        transform.localScale = new Vector3(GetWidth(points), 0.01f, GetLength(points));
    }

    public float GetWidth(Vector3[] points)
    {
        //Debug.Log("Width: "+GetDistance(points[0], points[1]));
        return GetDistance(points[0], points[1]);
    }

    public float GetLength(Vector3[] points)
    {
        //Debug.Log("length: " + GetDistance(points[1], points[2]));
        return GetDistance(points[1], points[2]);
    }

    private float GetDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }


    public void SetReferencePoints(out Vector3 center, out Vector3 forward)
    {
        Vector3[] boundaryPoints = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
        IgnoreY(boundaryPoints);

        center = CalculateCenter(boundaryPoints);
        forward = CalculateForward(center, boundaryPoints);

    }

    private void IgnoreY(Vector3[] points)
    {

        for (int i = 0; i < points.Length; i++)
        {
            points[i].y = 1;
            //Debug.Log("point " + i +": " + points[i]);
        }
    }

    public Vector3 CalculateCenter(Vector3[] points)
    {
        //get the sum of points and divide
        Vector3 center = Vector3.zero;
        foreach (Vector3 v in points)
        {
            center += v;
        }
        center = center / points.Length;
        return center;
    }

    private Vector3 CalculateForward(Vector3 center, Vector3[] points)
    {
        Vector3 forward = Vector3.zero;

        Vector3 firstEdge = (points[0] + points[1]) / 2;
        Vector3 secondEdge = (points[1] + points[2]) / 2;

        // the edge closest to the center is also the widest.
        if (Vector3.Distance(center, firstEdge) < Vector3.Distance(center, secondEdge))
        {
            forward = firstEdge;
        }
        else
        {
            forward = secondEdge;
        }
        return forward;
    }
}
