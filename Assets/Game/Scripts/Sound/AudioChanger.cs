using LogitechG29.Sample.Input;
using UnityEngine;
using UnityEngine.Audio;

public class AudioChanger : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private InputControllerReader inputControllerReader;

    private float volume = 0f;

    private void Start()
    {
        volume = PlayerPrefs.GetFloat("GameVolume", 0f);
        audioMixer.SetFloat("Volume", volume);
    }

    private void Update()
    {
        if (inputControllerReader.LeftBumper)
            ChangeVolume(-2f);
        if (inputControllerReader.RightBumper)
            ChangeVolume(+2f);
    }

    private void ChangeVolume(float deltaDb)
    {
        volume = Mathf.Clamp(volume + deltaDb, -80f, 0f);
        audioMixer.SetFloat("Volume", volume);

        PlayerPrefs.SetFloat("GameVolume", volume);
        PlayerPrefs.Save();
    }
}
