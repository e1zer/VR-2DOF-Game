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

    private Vector3 baseAngles;
    private bool baseSet = false;

    private void Awake()
    {
        sender = new SendingData();
        telemetry = sender.ObjectTelemetryData;
    }

    private void OnEnable()
    {
        telemetry.Reset();
        baseSet = false;
        telemetryRoutine = StartCoroutine(TelemetryLoop());
        sender.SendingStart();
    }

    private void OnDisable()
    {
        baseSet = false;
        telemetry?.Reset();

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
        if (!baseSet)
        {
            baseAngles = carTransform.eulerAngles;
            baseSet = true;
        }

        telemetry.Velocity = carRigidbody.velocity;

        Vector3 delta = NormalizeAngles(carTransform.eulerAngles - baseAngles);

        telemetry.Angles = new Vector3(delta.x, 0f, delta.z);
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
        angle %= 360f;
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}
