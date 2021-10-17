using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A Lidar-like radar for getting information about other objects nearby
/// See also https://en.wikipedia.org/wiki/Lidar
/// </summary>
public class Lidar : MonoBehaviour
{
    // how far are we looking?
    public float RayDistance = 50f;
    public bool ShowRayLines;

    /// <summary>
    /// Scan area using ray casts
    /// </summary>
    /// <returns>List of game objects within <see cref="RayDistance"/> from radar;</returns>
    public List<LidarScanObject> Scan(int fromAngle = -45, int toAngle = 45)
    {
        if (fromAngle > toAngle)
        {
            throw new ArgumentException($"{nameof(fromAngle)} must be smaller than {nameof(toAngle)}");
        }

        var scanObjects = new List<LidarScanObject>();
        var forwardVector = transform.forward;
        for (int angle = fromAngle; angle < toAngle; angle++)
        {
            // rotate around world space up, not local space (as car tilts a bit when turning)
            var scanVector = Quaternion.AngleAxis(angle, Vector3.up) * forwardVector;
            if (Physics.Raycast(transform.position, scanVector, out var hit, RayDistance))
            {
                var lidarObject = scanObjects.FirstOrDefault(o => o.gameObject == hit.collider.gameObject);
                if (lidarObject == null)
                {
                    scanObjects.Add(new LidarScanObject
                    {
                        gameObject = hit.collider.gameObject,
                        distance = hit.distance,
                        collider = hit.collider,
                        startAngle = angle
                    });
                }
                else
                {
                    lidarObject.endAngle = angle;
                }

                if (ShowRayLines)
                {
                    var t = transform;
                    Utils.DrawLine(t, t.position + scanVector * hit.distance, Color.yellow);
                }
            }
            else
            {
                if (ShowRayLines)
                {
                    var t = transform;
                    Utils.DrawLine(t, t.position + scanVector * RayDistance, Color.green);
                }
            }
        }

        return scanObjects;
    }

    public class LidarScanObject
    {
        public GameObject gameObject;
        public float distance;
        public Collider collider;
        public int startAngle;
        public int endAngle;
    }
}
