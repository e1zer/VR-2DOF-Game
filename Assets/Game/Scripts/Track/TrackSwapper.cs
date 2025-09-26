using TMPro;
using UnityEngine;

public class TrackSwapper : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Timer timer;
    [SerializeField]
    private Track[] tracks;
    [SerializeField]
    private TextMeshProUGUI bestTimeText;

    private Track currentTrack;

    private void Awake()
    {
        gameManager.OnEndGame += () => SaveBestTime(timer.ElapsedTime);
    }

    private void Start()
    {
        int indexTrack = Random.Range(0, tracks.Length);
        currentTrack = tracks[indexTrack];
        currentTrack.SetTrack();

        ShowBestTime();
    }

    private void ShowBestTime()
    {
        string trackKey = $"BestTime_{currentTrack.TrackName}";
        float bestTime = PlayerPrefs.GetFloat(trackKey, -1f);

        if (bestTime > 0f)
        {
            bestTimeText.text = $"{FormatTime(bestTime)}";
        }
        else
        {
            bestTimeText.text = "--:--";
        }
    }

    public void SaveBestTime(float newTime)
    {
        string trackKey = $"BestTime_{currentTrack.TrackName}";
        float bestTime = PlayerPrefs.GetFloat(trackKey, -1f);

        if (bestTime < 0f || newTime < bestTime)
        {
            PlayerPrefs.SetFloat(trackKey, newTime);
            PlayerPrefs.Save();
            ShowBestTime();
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return $"{minutes:00}:{seconds:00}";
    }
}

[System.Serializable]
public class Track
{
    [SerializeField] private string trackName; // имя трассы (например "Track1")
    [SerializeField] private GameObject[] walls;

    public string TrackName => trackName;

    public void SetTrack()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].SetActive(true);
        }
    }
}
