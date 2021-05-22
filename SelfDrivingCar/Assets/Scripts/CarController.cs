using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct Wheel
{
    public GameObject model;
    public WheelCollider collider;
}

public class CarController : MonoBehaviour
{
    public float MaxMotorTorque = 200.0f;
    public float MaxSteeringAngle = 30.0f;
    
    public List<Wheel> DriveWheels;
    public List<Wheel> SteeringWheels;
    
    public event EventHandler<float> SpeedChanged;

    float motorTorque;
    float brakeTorque;
    float steeringAngle;
    
    InputAction moveInputAction, brakeInputAction;
    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        rb.centerOfMass = new Vector3(0f, 0.5f, 0f);
        moveInputAction = GetComponent<PlayerInput>().actions.FindAction("Movement");
        brakeInputAction = GetComponent<PlayerInput>().actions.FindAction("Brake");
    }
    
    void Update()
    {
        // read speed input and set motor torque
        var movement = moveInputAction.ReadValue<Vector2>();
        motorTorque = Mathf.Lerp(motorTorque, movement.y * MaxMotorTorque, Time.deltaTime);
        
        // read brake input and set brake torque
        brakeTorque = brakeInputAction.ReadValue<float>() > 0f ? 1000 : 0;

        OnSpeedChanged();

        // if releasing key or turning the other direction, reset steeringAngle
        if (movement.x == 0
            || steeringAngle > 0 && movement.x < 0
            || steeringAngle < 0 && movement.x > 0) 
        {
            steeringAngle = 0;
        }
        
        steeringAngle = Mathf.Lerp(steeringAngle, movement.x * MaxSteeringAngle, Time.deltaTime);
        
        Move();
        Turn();
        AnimateWheels();
        OnSpeedChanged();
    }

    protected virtual void OnSpeedChanged ()
    {
        // report speed in km/h
        SpeedChanged?.Invoke (this, rb.velocity.magnitude * 3.6f);
    }

    void Move()
    {
        foreach (var wheel in DriveWheels)
        {
            wheel.collider.brakeTorque = brakeTorque;
        }

        if (brakeTorque > 0f)
        {
            motorTorque = 0f;
        }

        foreach (var wheel in DriveWheels)
        {
            wheel.collider.motorTorque = motorTorque;
        }
    }
    
    void Turn()
    {
        foreach (var wheel in SteeringWheels)
        {
            wheel.collider.steerAngle = steeringAngle;
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
