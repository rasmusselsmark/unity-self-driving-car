using System;
using UnityEngine;

public struct GroundScanResult
{
    public float Left { get; set; }
    public float Right { get; set; }
}

public class GroundRadar : MonoBehaviour
{
    public bool ShowRayLines;

    struct RoadRaycastHit
    {
        // angle to side of road
        public float Angle;

        // and distance from radar to where ray hit outside road (remember, this can be away from car)
        public float RoadDistance;
    }

    void Update()
    {
        Scan();
    }

    public GroundScanResult Scan()
    {
        // raytrace each side, until we don't hit road  
        var rightSideScan = FindSideOfRoadUsingRaycast(transform.forward, +1);
        var leftSideScan = FindSideOfRoadUsingRaycast(transform.forward, -1);

        var result = new GroundScanResult
        {
            Right = CalculateTriangleBaseSideLengthTan(rightSideScan.Angle, rightSideScan.RoadDistance),
            Left = CalculateTriangleBaseSideLengthTan(leftSideScan.Angle, leftSideScan.RoadDistance),
        };

        return result;
    }

    /// <summary>
    /// Returns angle and distance from radar to side of road. Either use -1 or +1 as increment to control if
    /// looking left or right.
    /// </summary>
    /// <param name="vector">Forward scan vector for radar.</param>
    /// <param name="increment">-1 to scan left side, +1 for right side.</param>
    /// <returns></returns>
    RoadRaycastHit FindSideOfRoadUsingRaycast(Vector3 vector, int increment)
    {
        var result = new RoadRaycastHit();
        var layerMask = LayerMask.GetMask("Road"); // only raycast road objects

        // first find distance to road at angle 0, i.e. adjacent side in our right-angled triangle
        // (where distance is opposite side)
        if (Physics.Raycast(transform.position, vector, out var hit, 20f, layerMask))
        {
            // remember last hit, so we can return when no longer hitting ground
            result.Angle = 0;
            result.RoadDistance = hit.distance;
        }
        else
        {
            // we're apparently outside road, return 0
            result.Angle = 0;
            result.RoadDistance = 0;

            return result;
        }

        // scan angles until we hit outside road
        var angle = 0;

        // TODO: There is a possible optimization here :)
        while (-85 < angle && angle < 85)
        {
            // rotate vector
            var scanVector = Quaternion.AngleAxis(angle, transform.up) * vector;
        
            if (Physics.Raycast(transform.position, scanVector, out hit, 20f, layerMask))
            {
                // remember last hit, so we can return when no longer hitting ground
                result.Angle = angle;

                if (ShowRayLines)
                {
                    Utils.DrawLine(transform, transform.position + scanVector * hit.distance, Color.green);
                }
            }
            else
            {
                if (ShowRayLines)
                {
                    Utils.DrawLine(transform, transform.position + scanVector * 5f, Color.yellow);
                }

                return result;
            }

            angle += increment;
        }

        // for now return 0 length if outside road, need to figure out how to best handle this situation
        return new RoadRaycastHit { Angle = angle, RoadDistance = 0.0f };
    }

    float CalculateTriangleBaseSideLengthSin(float degrees, float hypotenuse)
    {
        // see e.g. https://www.mathsisfun.com/algebra/trig-finding-side-right-triangle.html

        // we know angle and hypotenuse in right-angled triangle, and need to find opposite side (which is the
        // distance from side of road and end of scanning area)

        // sin(degrees) = opposite / hypotenuse
        // => sin(degrees) * hypotenuse = opposite
        
        var rad = degrees * Mathf.PI / 180; // Mathf.Sin uses radians instead of degrees
        return Mathf.Abs(Mathf.Sin(rad) * hypotenuse);
    }

    float CalculateTriangleBaseSideLengthTan(float degrees, float adjacent)
    {
        // tan(degrees) = opposite / adjacent
        // => oppsite = tan(degrees) * hypotenuse

        var rad = degrees * Mathf.PI / 180; // Mathf.Sin uses radians instead of degrees
        return Mathf.Abs(Mathf.Tan(rad) * adjacent);
    }
}
