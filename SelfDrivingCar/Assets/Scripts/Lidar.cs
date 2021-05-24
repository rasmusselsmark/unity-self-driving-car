using System;
using System.Collections.Generic;
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

    void FixedUpdate()
    {
        Scan();
    }

    /// <summary>
    /// Scan area using ray casts
    /// </summary>
    /// <returns>List of game objects within <see cref="RayDistance"/> from radar;</returns>
    public List<GameObject> Scan()
    {
        var objects = new List<GameObject>();

        var forwardVector = transform.forward;
        for (int angle = -45; angle < 45; angle++)
        {
            // rotate around world space up, not local space (as car tilts a bit when turning)
            var scanVector = Quaternion.AngleAxis(angle, Vector3.up) * forwardVector;
            if (Physics.Raycast(transform.position, scanVector, out var hit, RayDistance))
            {
                if (!objects.Contains(hit.collider.gameObject))
                {
                    objects.Add(hit.collider.gameObject);
                }

                if (ShowRayLines)
                {
                    Utils.DrawLine(transform, transform.position + scanVector * hit.distance, Color.yellow);
                }
            }
            else
            {
                if (ShowRayLines)
                {
                    Utils.DrawLine(transform, transform.position + scanVector * RayDistance, Color.green);
                }
            }
        }

        return objects;
    }
}
