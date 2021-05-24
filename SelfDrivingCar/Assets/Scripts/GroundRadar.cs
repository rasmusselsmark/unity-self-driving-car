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
    void Update()
    {
        Scan();
    }

    public GroundScanResult Scan()
    {
        // raytrace each side, until we don't hit road  
        var rightAngle = GetAngleWhenNoRoadRayTraceHit(-transform.up, +1);
        var leftAngle = GetAngleWhenNoRoadRayTraceHit(-transform.up, -1);

        var result = new GroundScanResult
        {
            Right = CalculateTriangleBaseSideLength(rightAngle),
            Left = CalculateTriangleBaseSideLength(leftAngle),
        };

        // print($"rightAngle: {rightAngle}; distance right: {result.Right}");
        return result;
    }

    float GetAngleWhenNoRoadRayTraceHit(Vector3 vector, int increment)
    {
        var angle = 0;

        while (-60 < angle && angle < 60)
        {
            // rotate vector
            var scanVector = Quaternion.AngleAxis(angle, transform.forward) * vector;
        
            var layerMask = LayerMask.GetMask("Road"); // only raycast road objects
            if (Physics.Raycast(transform.position, scanVector, out var hit, 5f, layerMask))
            {
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

                return angle;
            }

            angle += increment;
        }

        return angle;
    }

    float CalculateTriangleBaseSideLength(float degrees)
    {
        // see e.g. https://www.mathsisfun.com/algebra/trig-finding-side-right-triangle.html
        // use tangent as we know angle of B (so A = 90 - B) and opposite side in right-angled triangle
        // tan(A-angle) = a / b => tan(A-angle) * b = a => b = a / tan(A-angle)
        
        var rad = (90-degrees) * Mathf.PI / 180;
        return transform.position.y / Mathf.Tan(rad);
    }
}
