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
        // Подписываемся на все возможные кнопки Logitech G29
        input.OnEastButtonCallback += OnAnyButtonPressed;
        input.OnWestButtonCallback += OnAnyButtonPressed;
        input.OnNorthButtonCallback += OnAnyButtonPressed;
        input.OnSouthButtonCallback += OnAnyButtonPressed;
    }

    private void OnDestroy()
    {
        input.OnEastButtonCallback -= OnAnyButtonPressed;
        input.OnWestButtonCallback -= OnAnyButtonPressed;
        input.OnNorthButtonCallback -= OnAnyButtonPressed;
        input.OnSouthButtonCallback -= OnAnyButtonPressed;
    }

    private void OnAnyButtonPressed(bool isPress)
    {
        if (isPress && !isPressed)
        {
            isPressed = true;
            TransitionManager.Instance.LoadLevel("Track");
        }
    }
}
