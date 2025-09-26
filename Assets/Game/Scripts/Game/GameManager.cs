using LogitechG29.Sample.Input;
using MaskTransitions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio; // об€зательно дл€ AudioMixer

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputControllerReader inputControllerReader;
    [SerializeField] private int needLaps = 3;
    [SerializeField] private AudioMixer audioMixer; // сюда кидаешь AudioMixer из инспектора

    private bool isGameStarted = false;
    private int countLaps = 1;
    private float volume = 0f; // dB, лучше хранить именно dB, а не 0Ц1

    public int NeedLaps => needLaps;
    public bool IsGameStarted => isGameStarted;

    public event Action OnPreStart;
    public event Action OnStartGame;
    public event Action OnEndGame;
    public event Action<int> OnNextLap;

    private void Start()
    {
        // «агружаем сохранЄнную громкость (если нет, ставим 0 dB = 100%)
        volume = PlayerPrefs.GetFloat("GameVolume", 0f);
        audioMixer.SetFloat("Volume", volume);

        StartCoroutine(DelayStartGame());
    }

    private void Update()
    {
        if (inputControllerReader.EastButton && isGameStarted)
            RestartGame();

        if (inputControllerReader.LeftBumper)
            ChangeVolume(-2f); // уменьшаем на 2 дЅ
        if (inputControllerReader.RightBumper)
            ChangeVolume(+2f); // увеличиваем на 2 дЅ
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

    private void ChangeVolume(float deltaDb)
    {
        volume = Mathf.Clamp(volume + deltaDb, -80f, 0f);
        audioMixer.SetFloat("Volume", volume);

        PlayerPrefs.SetFloat("GameVolume", volume);
        PlayerPrefs.Save();
    }
}
