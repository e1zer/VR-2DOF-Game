using UnityEngine;

public class CreateComplexCollider : MonoBehaviour
{
    [Tooltip("Дополнительная высота для вертикальных капсул")]
    public float extraHeight = 2f;

    [Tooltip("Дополнительная ширина для широких объектов")]
    public float extraWidth = 0.5f;

    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var rend in renderers)
        {
            GameObject child = rend.gameObject;

            // Пропускаем, если коллайдер уже есть
            if (child.GetComponent<Collider>() != null)
                continue;

            Vector3 size = rend.bounds.size;

            if (size.y >= Mathf.Max(size.x, size.z))
            {
                // Высокий объект ? вертикальная капсула
                CapsuleCollider capsule = child.AddComponent<CapsuleCollider>();
                capsule.direction = 1; // Y
                capsule.height = size.y + extraHeight;
                capsule.radius = Mathf.Max(size.x, size.z) / 2f;
                capsule.center = child.transform.InverseTransformPoint(rend.bounds.center);
            }
            else
            {
                // Широкий объект ? BoxCollider
                BoxCollider box = child.AddComponent<BoxCollider>();
                Vector3 boxSize = size;
                boxSize.x += extraWidth;
                boxSize.z = 1f;
                box.size = boxSize;
                box.center = child.transform.InverseTransformPoint(rend.bounds.center);
            }
        }
    }
}
