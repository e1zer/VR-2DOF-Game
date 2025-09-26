#region

using LogitechG29.Sample.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class CarController : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Rigidbody rigidbody;
    [SerializeField]
    private InputControllerReader inputControllerReader;
    [SerializeField]
    public List<AxleInfo> axleInfos;

    [SerializeField]
    private float maxMotorTorque = 1500f;
    [SerializeField]
    private float maxSteeringAngle = 30f;
    [SerializeField]
    private float maxSpeed = 50f;

    private float throttleInput;
    private float brakeInput;

    public float GetThrottle() => throttleInput;
    public float GetBrake() => brakeInput;

    public float MaxSpeed => maxSpeed;

    private Action<float> throttleHandler;
    private Action<float> brakeHandler;

    private void Awake()
    {
        throttleHandler = (val) => throttleInput = val;
        brakeHandler = (val) => brakeInput = val;

        inputControllerReader.ThrottleCallback += throttleHandler;
        inputControllerReader.BrakeCallback += brakeHandler;
        inputControllerReader.SteeringCallback += Steering;

        gameManager.OnEndGame += FinishBrake;
    }

    private void OnDestroy()
    {
        inputControllerReader.ThrottleCallback -= throttleHandler;
        inputControllerReader.BrakeCallback -= brakeHandler;
        inputControllerReader.SteeringCallback -= Steering;

        gameManager.OnEndGame -= FinishBrake;
    }

    private void Start()
    {
        rigidbody.centerOfMass = new Vector3(0f, 0.3f, -0.10f);
    }

    private void FixedUpdate()
    {
        if (gameManager.IsGameStarted)
            ApplyThrottle(throttleInput, brakeInput);
    }

    private void Steering(float steeringInput)
    {
        var steering = maxSteeringAngle * steeringInput;

        foreach (var axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
        }
    }

    private void ApplyThrottle(float throttle, float brake)
    {
        float localForwardVel = transform.InverseTransformDirection(rigidbody.velocity).z;

        foreach (var axleInfo in axleInfos)
        {
            if (!axleInfo.motor)
                continue;

            float motor = 0f;
            float appliedBrake = 0f;

            if (throttle > 0.1f)
            {
                if (rigidbody.velocity.magnitude < maxSpeed)
                    motor = maxMotorTorque * throttle;

                if (localForwardVel < -0.1f)
                    appliedBrake = 3000f;
            }

            if (brake > 0.1f)
            {
                if (localForwardVel > 0.1f)
                    appliedBrake = 3000f;

                if (localForwardVel < 0.1f)
                    motor = -maxMotorTorque * brake;
            }

            axleInfo.leftWheel.motorTorque = motor;
            axleInfo.rightWheel.motorTorque = motor;

            axleInfo.leftWheel.brakeTorque = appliedBrake;
            axleInfo.rightWheel.brakeTorque = appliedBrake;
        }
    }

    private void FinishBrake()
    {
        foreach (var axleInfo in axleInfos)
        {
            axleInfo.leftWheel.brakeTorque = 3000f;
            axleInfo.rightWheel.brakeTorque = 3000f;
        }
    }
}

[Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
