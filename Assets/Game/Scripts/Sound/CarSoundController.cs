using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarSoundController : MonoBehaviour
{
    [SerializeField] private CarController carController;
    [SerializeField] private Rigidbody rb;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource fadeSource;

    [Header("Engine Sounds")]
    [SerializeField] private AudioClip startup;
    [SerializeField] private AudioClip idle;
    [SerializeField] private AudioClip low_on, low_off;

    [Header("Engine Settings")]
    [SerializeField] private float idleRPM = 800f;
    [SerializeField] private float maxRPM = 4000f;
    [SerializeField] private float rpmIncreaseRate = 2500f;
    [SerializeField] private float rpmDecreaseRate = 2000f;

    [Header("Gears")]
    [SerializeField] private int currentGear = 1;

    private int maxGear = 5;
    private float[] gearSpeedLimits = { 9f, 18f, 24f, 27f, 38f };

    private bool isEngineStarted = false;
    private float currentRPM;
    private float throttleInput;

    private void Start()
    {
        currentRPM = idleRPM;
        PlayStartup();
    }

    private void Update()
    {
        if (!isEngineStarted) return;

        throttleInput = Mathf.Max(0f, carController.GetThrottle());
        float speed = rb.velocity.magnitude;

        UpdateGear(speed);
        UpdateRPM(speed, throttleInput);

        AudioClip targetClip = SelectClip();
        CrossfadeTo(targetClip);

        float rpm01 = Mathf.InverseLerp(idleRPM, maxRPM, currentRPM);
        mainSource.pitch = Mathf.Lerp(0.8f, 1.1f, rpm01);
    }

    private void UpdateGear(float speed)
    {
        if (currentGear < maxGear && speed > gearSpeedLimits[currentGear - 1])
        {
            currentGear++;
            GearShift();
        }
        else if (currentGear > 1 && speed < gearSpeedLimits[currentGear - 2] * 0.8f)
        {
            currentGear--;
            GearShift();
        }
    }

    private void GearShift()
    {
        // сброс RPM
        currentRPM *= 0.6f;

        // имитация переключения через корутину
        StopCoroutine(nameof(GearShiftEffect));
        StartCoroutine(GearShiftEffect());
    }

    private IEnumerator GearShiftEffect()
    {
        float originalPitch = mainSource.pitch;
        float originalVolume = mainSource.volume;

        // короткий провал
        mainSource.pitch *= 0.6f;
        mainSource.volume *= 0.6f;

        yield return new WaitForSeconds(0.15f);

        // плавное возвращение
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            mainSource.pitch = Mathf.Lerp(mainSource.pitch, originalPitch, t);
            mainSource.volume = Mathf.Lerp(mainSource.volume, originalVolume, t);
            yield return null;
        }
    }

    private void UpdateRPM(float speed, float throttle)
    {
        if (throttle > 0.1f)
            currentRPM += rpmIncreaseRate * throttle * Time.deltaTime;
        else
            currentRPM -= rpmDecreaseRate * Time.deltaTime;

        float gearMaxRPM = Mathf.Lerp(idleRPM + 1000f, maxRPM, (float)currentGear / maxGear);
        currentRPM = Mathf.Clamp(currentRPM, idleRPM, gearMaxRPM);
    }

    private AudioClip SelectClip()
    {
        if (currentRPM < 1200f)
            return idle;
        else
            return throttleInput > 0.1f ? low_on : low_off;
    }

    private void PlayStartup()
    {
        mainSource.loop = false;
        mainSource.clip = startup;
        mainSource.Play();
        Invoke(nameof(StartEngine), startup.length);
    }

    private void StartEngine()
    {
        isEngineStarted = true;
        CrossfadeTo(idle);
    }

    private void CrossfadeTo(AudioClip clip)
    {
        if (mainSource.clip == clip) return;

        fadeSource.clip = mainSource.clip;
        fadeSource.volume = mainSource.volume;
        fadeSource.pitch = mainSource.pitch;
        fadeSource.loop = true;
        fadeSource.Play();

        mainSource.clip = clip;
        mainSource.loop = true;
        mainSource.volume = 0f;
        mainSource.Play();

        StopAllCoroutines();
        StartCoroutine(FadeTransition());
    }

    private IEnumerator FadeTransition()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            mainSource.volume = Mathf.Lerp(0f, 0.1f, t);
            fadeSource.volume = Mathf.Lerp(0.1f, 0f, t);
            yield return null;
        }
        fadeSource.Stop();
    }
}
