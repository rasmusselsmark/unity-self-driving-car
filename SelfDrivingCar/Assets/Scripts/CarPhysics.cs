using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Wheel
{
    public GameObject model;
    public WheelCollider collider;
}

public class CarPhysics : MonoBehaviour
{
    public float MaxMotorTorque = 200.0f;
    public float MaxSteeringAngle = 30.0f;
    
    // these are the "desired" values for motor, brake and steering
    // corresponding Internal* fields has the actual values, using Mathf.Lerp() to smoothen
    internal float MotorTorque;
    internal float SteeringAngle;

    internal float BrakeTorque;

    public List<Wheel> DriveWheels;
    public List<Wheel> SteeringWheels;
    
    float InternalMotorTorque;
    float InternalSteeringAngle;

    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        rb.centerOfMass = new Vector3(0f, 0.5f, 0f);
    }
    
    void LateUpdate()
    {
        InternalMotorTorque = Mathf.Lerp(InternalMotorTorque, MotorTorque, Time.deltaTime);

        // if releasing key or turning the other direction, reset steeringAngle
        if (SteeringAngle == 0
            || SteeringAngle > 0 && InternalSteeringAngle < 0
            || SteeringAngle < 0 && InternalSteeringAngle > 0)
        {
            InternalSteeringAngle = 0;
        }
        else
        {
            InternalSteeringAngle = Mathf.Lerp(
                InternalSteeringAngle,
                Mathf.Clamp(SteeringAngle, -MaxSteeringAngle, MaxSteeringAngle),
                Time.deltaTime);
        }

        // print($"MotorTorque: {MotorTorque}; Speed: {Speed}");

        Move();
        Turn();
        AnimateWheels();
    }

    /// <summary>
    /// Speed in km/h
    /// </summary>
    public float Speed => rb.velocity.magnitude * 3.6f;

    void Move()
    {
        foreach (var wheel in DriveWheels)
        {
            wheel.collider.brakeTorque = BrakeTorque;
        }

        // don't speed up if we're braking car
        if (BrakeTorque > 0f)
        {
            MotorTorque = 0f;
            InternalMotorTorque = 0f;
        }

        foreach (var wheel in DriveWheels)
        {
            wheel.collider.motorTorque = InternalMotorTorque;
        }
    }
    
    void Turn()
    {
        foreach (var wheel in SteeringWheels)
        {
            wheel.collider.steerAngle = InternalSteeringAngle;
        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in DriveWheels)
        {
            RotateAndPosition(wheel);
        }

        foreach (var wheel in SteeringWheels)
        {
            RotateAndPosition(wheel);
        }

        void RotateAndPosition(Wheel wheel)
        {
            wheel.collider.GetWorldPose(out var position, out var rotation);
            wheel.model.transform.position = position;
            wheel.model.transform.rotation = rotation;
        }
    }
}
