using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Timer timer;
    [SerializeField] private TMP_Text timerText;

    private void OnEnable()
    {
        if (timer != null)
            timer.OnTimeChanged += UpdateTimerText;
    }

    private void OnDisable()
    {
        if (timer != null)
            timer.OnTimeChanged -= UpdateTimerText;
    }

    private void UpdateTimerText(int minutes, int seconds)
    {
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
