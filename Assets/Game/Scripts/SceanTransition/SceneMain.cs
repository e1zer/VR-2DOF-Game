using MaskTransitions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class SceneMain : MonoBehaviour
{
    private void Awake()
    {
        InputSystem.onAnyButtonPress.CallOnce(OnAnyButton);
    }


    private void OnAnyButton(InputControl control)
    {
        TransitionManager.Instance.LoadLevel("Track");
    }
}
