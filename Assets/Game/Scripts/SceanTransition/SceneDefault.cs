using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class SceneDefault : MonoBehaviour, IScene
{
    [SerializeField]
    protected SceneSwapper swapper;

    private CanvasGroup canvasGroup;

    public event Action<IScene> OnShown;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetVisible(bool isShowed, float duration = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(isShowed, duration));
    }

    private IEnumerator Fade(bool isShowed, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float endAlpha = isShowed ? 1f : 0f;
        float time = 0;

        canvasGroup.blocksRaycasts = isShowed;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            yield return null;
        }

        canvasGroup.interactable = isShowed;
        canvasGroup.alpha = isShowed ? 1 : 0;

        if (isShowed)
            OnShown?.Invoke(this);
    }
}
