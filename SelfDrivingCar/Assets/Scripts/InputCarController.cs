using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputCarController : MonoBehaviour
{
    InputAction moveInputAction, brakeInputAction;
    CarPhysics m_CarPhysics;

    // Start is called before the first frame update
    void Start()
    {
        m_CarPhysics = GetComponent<CarPhysics>();
        moveInputAction = GetComponent<PlayerInput>().actions.FindAction("Movement");
        brakeInputAction = GetComponent<PlayerInput>().actions.FindAction("Brake");
    }

    // Update is called once per frame
    void Update()
    {
        // read speed input and set motor torque
        var movement = moveInputAction.ReadValue<Vector2>();
        m_CarPhysics.MotorTorque = movement.y * m_CarPhysics.MaxMotorTorque;
        
        // read brake input and set brake torque
        m_CarPhysics.BrakeTorque = brakeInputAction.ReadValue<float>() > 0f ? 1000 : 0;

        // if releasing key or turning the other direction, reset steeringAngle
        if (movement.x == 0
            || m_CarPhysics.SteeringAngle > 0 && movement.x < 0
            || m_CarPhysics.SteeringAngle < 0 && movement.x > 0) 
        {
            m_CarPhysics.SteeringAngle = 0;
        }
        else
        {
            m_CarPhysics.SteeringAngle = movement.x * m_CarPhysics.MaxSteeringAngle;
        }
    }
}
