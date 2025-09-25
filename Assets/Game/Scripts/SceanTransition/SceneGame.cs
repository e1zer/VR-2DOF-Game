using MaskTransitions;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneGame : MonoBehaviour
{
    private void Awake()
    {

    }


    private void OnAnyButton(InputControl control)
    {
        TransitionManager.Instance.LoadLevel("Track");
    }
}
