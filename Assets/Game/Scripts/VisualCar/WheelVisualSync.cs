using UnityEngine;

public class WheelVisualSync : MonoBehaviour
{
    [System.Serializable]
    public class Wheel
    {
        public WheelCollider collider;
        public Transform mesh;
    }

    public Wheel[] wheels;

    private void Update()
    {
        foreach (var wheel in wheels)
        {
            UpdateWheelPose(wheel.collider, wheel.mesh);
        }
    }

    private void UpdateWheelPose(WheelCollider collider, Transform mash)
    {
        Quaternion rotation;
        collider.GetWorldPose(out _, out rotation);

        mash.rotation = rotation;
    }
}
