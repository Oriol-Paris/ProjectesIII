using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundFXVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private void Start()
    {
        if (masterVolumeSlider != null && soundFXVolumeSlider != null && musicVolumeSlider != null)
        {
            // Set slider min and max values
            masterVolumeSlider.minValue = -80f;
            masterVolumeSlider.maxValue = 20f;
            soundFXVolumeSlider.minValue = -80f;
            soundFXVolumeSlider.maxValue = 20f;
            musicVolumeSlider.minValue = -80f;
            musicVolumeSlider.maxValue = 20f;

            // Add listeners to sliders to call respective methods when value changes
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            soundFXVolumeSlider.onValueChanged.AddListener(SetSoundFXVolume);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

            // Initialize slider values based on current mixer values
            float masterVolume;
            audioMixer.GetFloat("MasterVolume", out masterVolume);
            masterVolumeSlider.value = masterVolume;

            float soundFXVolume;
            audioMixer.GetFloat("soundFXVolume", out soundFXVolume);
            soundFXVolumeSlider.value = soundFXVolume;

            float musicVolume;
            audioMixer.GetFloat("MusicVolume", out musicVolume);
            musicVolumeSlider.value = musicVolume;
        }
    }

    private void Update()
    {
        if(masterVolumeSlider != null&&soundFXVolumeSlider!=null&&musicVolumeSlider!=null) { 
        SetMasterVolume(masterVolumeSlider.value);
        SetSoundFXVolume(soundFXVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        }
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", level);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", level);
    }
}
