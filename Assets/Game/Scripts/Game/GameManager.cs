using LogitechG29.Sample.Input;
using MaskTransitions;
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private InputControllerReader inputControllerReader;

    private bool isGameStarted = false;

    public bool IsGameStarted => isGameStarted;

    public event Action OnPreStart;
    public event Action OnStartGame;
    public event Action OnEndGame;

    private void Start()
    {
        StartCoroutine(DelayStartGame());
    }

    private void Update()
    {
        if (inputControllerReader.EastButton && isGameStarted)
            RestartGame();
    }

    private void StartGame()
    {
        isGameStarted = true;
        OnStartGame?.Invoke();
    }

    private void EndGame()
    {
        isGameStarted = false;
        OnEndGame?.Invoke();
    }

    private void RestartGame()
    {
        isGameStarted = false;
        TransitionManager.Instance.LoadLevel("MainMenuTrack");
    }

    private IEnumerator DelayStartGame()
    {
        yield return new WaitForSeconds(1f);
        OnPreStart?.Invoke();
        yield return new WaitForSeconds(1f);
        OnPreStart?.Invoke();
        yield return new WaitForSeconds(1f);
        OnPreStart?.Invoke();
        yield return new WaitForSeconds(1f);
        StartGame();
    }
}
