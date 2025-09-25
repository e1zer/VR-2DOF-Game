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

    private void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        //mesh.position = position;
        mesh.rotation = rotation;
    }
}
