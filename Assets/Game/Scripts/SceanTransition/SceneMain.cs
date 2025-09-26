using LogitechG29.Sample.Input;
using MaskTransitions;
using UnityEngine;

public class SceneMain : MonoBehaviour
{
    [SerializeField]
    private InputControllerReader input;

    private bool isPressed = false;

    private void Awake()
    {
        input.OnEastButtonCallback += PressStart;
    }

    private void OnDestroy()
    {
        input.OnEastButtonCallback -= PressStart;
    }

    private void PressStart(bool isPress)
    {
        if (isPress && !isPressed)
        {
            TransitionManager.Instance.LoadLevel("Track");
            isPressed = true;
        }
    }
}
