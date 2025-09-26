using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private GameManager manager;

    private float elapsedTime = 0f;
    private bool isRunning = false;

    public float ElapsedTime => elapsedTime;

    public event Action<int, int> OnTimeChanged;

    private void Awake()
    {
        manager.OnStartGame += StartTimer;
        manager.OnEndGame += StopTimer;
    }

    private void OnDestroy()
    {
        manager.OnStartGame -= StartTimer;
        manager.OnEndGame -= StopTimer;
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            OnTimeChanged?.Invoke(minutes, seconds);
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        OnTimeChanged?.Invoke(0, 0);
    }
}
