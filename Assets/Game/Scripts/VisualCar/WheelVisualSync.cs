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

    void Update()
    {
        foreach (var w in wheels)
        {
            UpdateWheelPose(w.collider, w.mesh);
        }
    }

    void UpdateWheelPose(WheelCollider col, Transform mash)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);

        rot.y = 0f;

        mash.position = pos;
        mash.rotation = rot;
    }
}
