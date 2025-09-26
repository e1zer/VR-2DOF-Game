using _2DOF;
using System.Collections;
using UnityEngine;

public class CarTelemetryManager : MonoBehaviour
{
    [SerializeField]
    private Transform carTransform;
    [SerializeField]
    private Rigidbody carRigidbody;

    private SendingData sender;
    private ObjectTelemetryData telemetry;

    private Coroutine telemetryRoutine;

    private void Awake()
    {
        sender = new SendingData();
        telemetry = sender.ObjectTelemetryData;
    }

    private void OnEnable()
    {
        telemetryRoutine = StartCoroutine(TelemetryLoop());
        sender.SendingStart();
    }

    private void OnDisable()
    {
        if (telemetryRoutine != null)
        {
            StopCoroutine(telemetryRoutine);
            telemetryRoutine = null;
        }
        sender.SendingStop();
    }

    private IEnumerator TelemetryLoop()
    {
        var delay = new WaitForSeconds(SendingData.WAIT_TIME / 1000f);

        while (true)
        {
            if (telemetry != null)
            {
                UpdateTelemetry();
            }
            else
            {
                yield return new WaitForSeconds(SendingData.WAIT_TIME / 100f);
            }

            yield return delay;
        }
    }

    private void UpdateTelemetry()
    {
        telemetry.Velocity = carRigidbody.velocity;
        telemetry.Angles = NormalizeAngles(carTransform.eulerAngles);
    }

    private Vector3 NormalizeAngles(Vector3 euler)
    {
        return new Vector3(
            NormalizeAxis(euler.x),
            NormalizeAxis(euler.y),
            NormalizeAxis(euler.z)
        );
    }

    private float NormalizeAxis(float angle)
    {
        if (Mathf.Approximately(angle, 180f))
            return 0f;

        return angle > 180f ? angle - 360f : angle;
    }
}
