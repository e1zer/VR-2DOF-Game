using Cinemachine;
using UnityEngine;

public class FreeLookCameraSwitcher : MonoBehaviour
{
    public CinemachineFreeLook[] cameras; // Сюда закинь все свои FreeLook камеры
    public int defaultIndex = 0; // Какая камера активна по умолчанию
    public int activePriority = 20; // Приоритет активной камеры
    public int inactivePriority = 0; // Приоритет неактивных камер

    private int currentIndex;

    void Start()
    {
        SetActiveCamera(defaultIndex);
    }

    void Update()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString())) // клавиши 1,2,3...
            {
                SetActiveCamera(i);
            }
        }
    }

    private void SetActiveCamera(int index)
    {
        if (index < 0 || index >= cameras.Length) return;

        currentIndex = index;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].Priority = (i == currentIndex) ? activePriority : inactivePriority;
        }
    }
}
