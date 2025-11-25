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

        ApplyAntiRollBar();
        LimitGripOnSteepSurfaces();
        AdjustWheelGripBySpeed();
    }



    private void LimitGripOnSteepSurfaces()
    {
        foreach (var axleInfo in axleInfos)
        {
            LimitWheelFriction(axleInfo.leftWheel);
            LimitWheelFriction(axleInfo.rightWheel);
        }
    }

    private void LimitWheelFriction(WheelCollider wheel)
    {
        if (wheel.GetGroundHit(out WheelHit hit))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            if (angle > 40f)
            {
                // Ослабляем сцепление при езде по стене
                var forwardFriction = wheel.forwardFriction;
                var sidewaysFriction = wheel.sidewaysFriction;

                forwardFriction.stiffness = 0.2f;
                sidewaysFriction.stiffness = 0.2f;

                wheel.forwardFriction = forwardFriction;
                wheel.sidewaysFriction = sidewaysFriction;
            }
            else
            {
                // Восстанавливаем нормальное сцепление
                var forwardFriction = wheel.forwardFriction;
                var sidewaysFriction = wheel.sidewaysFriction;

                forwardFriction.stiffness = 1.0f;
                sidewaysFriction.stiffness = 1.0f;

                wheel.forwardFriction = forwardFriction;
                wheel.sidewaysFriction = sidewaysFriction;
            }
        }
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

    private void ApplyAntiRollBar()
    {
        foreach (var axleInfo in axleInfos)
        {
            WheelHit hitL;
            WheelHit hitR;
            float travelL = 1.0f;
            float travelR = 1.0f;

            bool groundedL = axleInfo.leftWheel.GetGroundHit(out hitL);
            bool groundedR = axleInfo.rightWheel.GetGroundHit(out hitR);

            if (groundedL)
                travelL = (-axleInfo.leftWheel.transform.InverseTransformPoint(hitL.point).y - axleInfo.leftWheel.radius) / axleInfo.leftWheel.suspensionDistance;
            if (groundedR)
                travelR = (-axleInfo.rightWheel.transform.InverseTransformPoint(hitR.point).y - axleInfo.rightWheel.radius) / axleInfo.rightWheel.suspensionDistance;

            float antiRollForce = (travelL - travelR) * 5000f; // 5000f можно подбирать под твой вес машины

            if (groundedL)
                rigidbody.AddForceAtPosition(axleInfo.leftWheel.transform.up * -antiRollForce, axleInfo.leftWheel.transform.position);
            if (groundedR)
                rigidbody.AddForceAtPosition(axleInfo.rightWheel.transform.up * antiRollForce, axleInfo.rightWheel.transform.position);
        }
    }

    private void AdjustWheelGripBySpeed()
    {
        float speed = rigidbody.velocity.magnitude;

        // Рассчитываем множитель сцепления в зависимости от скорости
        float gripMultiplier = Mathf.Lerp(1.5f, 0.8f, speed / maxSpeed);

        foreach (var axleInfo in axleInfos)
        {
            AdjustWheelGrip(axleInfo.leftWheel, gripMultiplier);
            AdjustWheelGrip(axleInfo.rightWheel, gripMultiplier);
        }
    }

    private void AdjustWheelGrip(WheelCollider wheel, float gripMultiplier)
    {
        WheelFrictionCurve forward = wheel.forwardFriction;
        WheelFrictionCurve sideways = wheel.sidewaysFriction;

        forward.stiffness = gripMultiplier;
        sideways.stiffness = gripMultiplier * 1.2f; // немного больше для бокового сцепления

        wheel.forwardFriction = forward;
        wheel.sidewaysFriction = sideways;
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
