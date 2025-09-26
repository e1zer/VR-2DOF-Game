using System.Collections;
using TMPro;
using UnityEngine;

public class CarDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI lapText;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private float showTime = 2f;
    [SerializeField]
    private float fadeDuration = 0.5f;

    private Coroutine showCoroutine;

    private void OnEnable()
    {
        gameManager.OnNextLap += HandleNextLap;
    }

    private void OnDisable()
    {
        gameManager.OnNextLap -= HandleNextLap;
    }

    private void HandleNextLap(int currentLap)
    {
        if (showCoroutine != null) StopCoroutine(showCoroutine);
        showCoroutine = StartCoroutine(ShowLapRoutine(currentLap));
    }

    private IEnumerator ShowLapRoutine(int currentLap)
    {
        lapText.text = $"Круг {currentLap}/{gameManager.NeedLaps}";
        Color startColor = lapText.color;
        startColor.a = 1f;
        lapText.color = startColor;

        yield return new WaitForSeconds(showTime);

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            lapText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        lapText.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }
}
