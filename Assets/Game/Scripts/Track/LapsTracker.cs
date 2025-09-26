using System.Collections.Generic;
using UnityEngine;

public class LapsTracker : MonoBehaviour
{
    [SerializeField]
    private GameManager manager;
    [SerializeField]
    private LapTrigger[] triggers;
    [SerializeField]
    private LapTrigger finishTrigger;

    private List<int> usedTrackers = new List<int>();

    private void Awake()
    {
        foreach (LapTrigger trigger in triggers)
        {
            trigger.OnTriggerExitUsed += ActivateTrigger;
        }

        finishTrigger.OnTriggerExitUsed += LapFinished;
    }

    private void OnDestroy()
    {
        foreach (LapTrigger trigger in triggers)
        {
            trigger.OnTriggerExitUsed -= ActivateTrigger;
        }

        finishTrigger.OnTriggerExitUsed -= LapFinished;
    }

    private void LapFinished(int _)
    {
        if (usedTrackers.Count >= 3)
        {
            manager.NextLaps();
            RestartLap();
        }
    }

    private void ActivateTrigger(int index)
    {
        Debug.Log($"Index триннер: {index}");

        if (usedTrackers.Count == 0)
        {
            usedTrackers.Add(index);
            return;
        }

        int newIndex = 0;

        foreach (int indexTrigger in usedTrackers)
        {
            if (indexTrigger == index)
                return;

            newIndex = index;
        }

        usedTrackers.Add(newIndex);
    }

    private void RestartLap()
    {
        usedTrackers = new List<int>();
    }
}
