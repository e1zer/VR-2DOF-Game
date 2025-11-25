using LogitechG29.Sample.Input;
using MaskTransitions;
using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private InputControllerReader inputControllerReader;
    [SerializeField]
    private int needLaps = 3;

    private bool isGameStarted = false;
    private int countLaps = 1;

    public int NeedLaps => needLaps;
    public bool IsGameStarted => isGameStarted;

    public event Action OnPreStart;
    public event Action OnStartGame;
    public event Action OnEndGame;
    public event Action<int> OnNextLap;

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
        StartCoroutine(BackToMainMenu());
        OnEndGame?.Invoke();
    }

    public void NextLaps()
    {
        if (needLaps <= countLaps)
        {
            EndGame();
        }
        else
        {
            countLaps++;
            OnNextLap?.Invoke(countLaps);
        }
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

    private IEnumerator BackToMainMenu()
    {
        yield return new WaitForSeconds(5f);
        RestartGame();
    }
}
