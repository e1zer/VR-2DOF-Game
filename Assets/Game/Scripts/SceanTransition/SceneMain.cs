using MaskTransitions;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneMain : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            TransitionManager.Instance.LoadLevel("Track");
        }
    }
}
