#region

using LogitechG29.Sample.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class CarController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigidbody;
    [SerializeField]
    private InputControllerReader inputControllerReader;
    [SerializeField]
    private List<AxleInfo> axleInfos; // информация о каждой отдельной оси

    [SerializeField]
    private float maxMotorTorque; // максимальный крутящий момент, который двигатель может приложить к колесу

    [SerializeField]
    private float maxSteeringAngle; // максимальный угол поворота, который может иметь колесо

    private void Awake()
    {
        inputControllerReader.ThrottleCallback += Throttle;
        inputControllerReader.BrakeCallback += Brake;
        inputControllerReader.SteeringCallback += Steering;
    }

    private void Start()
    {
        rigidbody.centerOfMass = new Vector3(0f, -0.5f, 0f);
    }

    private void OnDestroy()
    {
        inputControllerReader.ThrottleCallback -= Throttle;
        inputControllerReader.BrakeCallback -= Brake;
        inputControllerReader.SteeringCallback -= Steering;
    }

    private void Steering(float steeringInput)
    {
        var steering = maxSteeringAngle * inputControllerReader.Steering;

        foreach (var axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
                //Debug.Log(steering);
            }
        }
    }

    private void Brake(float valueInput)
    {
        float localForwardVel = transform.InverseTransformDirection(rigidbody.velocity).z;
        bool isBraking = valueInput > 0.1f;

        foreach (var axleInfo in axleInfos)
        {
            // Тормозим ВСЕ колёса
            float brake = 0f;
            if (isBraking && localForwardVel > 0.1f) brake = 3000f; // едем вперед → тормоз
            axleInfo.leftWheel.brakeTorque = brake;
            axleInfo.rightWheel.brakeTorque = brake;

            if (!axleInfo.motor) continue;

            // Движение задним ходом
            if (isBraking && Mathf.Abs(localForwardVel) <= 0.1f)
            {
                axleInfo.leftWheel.motorTorque = -maxMotorTorque * valueInput;
                axleInfo.rightWheel.motorTorque = -maxMotorTorque * valueInput;
            }
            // Если едем назад и тормоза нет → плавное движение
            else if (localForwardVel < -0.1f)
            {
                axleInfo.leftWheel.motorTorque = -maxMotorTorque * 0.5f;
                axleInfo.rightWheel.motorTorque = -maxMotorTorque * 0.5f;
            }
            else
            {
                axleInfo.leftWheel.motorTorque = 0f;
                axleInfo.rightWheel.motorTorque = 0f;
            }
        }
    }

    private void Throttle(float valueInput)
    {
        float localForwardVel = transform.InverseTransformDirection(rigidbody.velocity).z;
        bool isAccelerating = valueInput > 0.1f;

        foreach (var axleInfo in axleInfos)
        {
            // Газ влияет только на моторные колеса
            if (!axleInfo.motor) continue;

            // Если едем назад и нажали газ → тормоз и газ вперед
            if (isAccelerating && localForwardVel < -0.1f)
            {
                // Тормозим все колеса
                foreach (var brakeAxle in axleInfos)
                {
                    brakeAxle.leftWheel.brakeTorque = 3000f;
                    brakeAxle.rightWheel.brakeTorque = 3000f;
                }

                axleInfo.leftWheel.motorTorque = maxMotorTorque * valueInput;
                axleInfo.rightWheel.motorTorque = maxMotorTorque * valueInput;
            }
            // Обычное ускорение вперед
            else if (isAccelerating)
            {
                // Снимаем тормоз со всех колес
                foreach (var brakeAxle in axleInfos)
                {
                    brakeAxle.leftWheel.brakeTorque = 0f;
                    brakeAxle.rightWheel.brakeTorque = 0f;
                }

                axleInfo.leftWheel.motorTorque = maxMotorTorque * valueInput;
                axleInfo.rightWheel.motorTorque = maxMotorTorque * valueInput;
            }
            else
            {
                axleInfo.leftWheel.motorTorque = 0f;
                axleInfo.rightWheel.motorTorque = 0f;
            }
        }
    }

}


[Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // это колесо прикреплено к мотору?
    public bool steering; // применяет ли это колесо угол поворота?
}
