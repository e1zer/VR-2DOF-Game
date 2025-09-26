using System;
using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    [SerializeField]
    private int index = 0;

    public int Index => index;

    public event Action<int> OnTriggerExitUsed;

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<CarController>())
        {
            OnTriggerExitUsed?.Invoke(index);
        }
    }
}
