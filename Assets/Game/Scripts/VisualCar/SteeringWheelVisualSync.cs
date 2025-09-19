using LogitechG29.Sample.Input;
using UnityEngine;

public class SteeringWheelVisualSync : MonoBehaviour
{
    [SerializeField]
    private InputControllerReader inputControllerReader;
    [SerializeField]
    private Transform steeringWheel;

    private void Update()
    {
        UpdateSteeringWheel();
    }

    private void UpdateSteeringWheel()
    {
        Quaternion rotation;

        rotation = steeringWheel.localRotation;

        rotation.z = -inputControllerReader.Steering;

        steeringWheel.localRotation = rotation;
    }
}
