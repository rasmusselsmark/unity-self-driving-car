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
        // read speed input and set motor torque plus steering angle
        var movement = moveInputAction.ReadValue<Vector2>();
        m_CarPhysics.MotorTorque = movement.y * m_CarPhysics.MaxMotorTorque;
        m_CarPhysics.SteeringAngle = movement.x * m_CarPhysics.MaxSteeringAngle;

        // read brake input and set brake torque
        m_CarPhysics.BrakeTorque = brakeInputAction.ReadValue<float>() > 0f ? 1000 : 0;
    }
}
