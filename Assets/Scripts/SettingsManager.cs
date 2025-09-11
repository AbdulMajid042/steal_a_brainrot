using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider soundSlider;
    public Slider musicSlider;
    public Slider sensitivitySlider;

    private void Start()
    {
        // Load saved values, if they exist (default to 1 for sound/music, 0.5 for sensitivity)
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 0.5f);

        // Apply immediately
        ApplySound(soundSlider.value);
        ApplyMusic(musicSlider.value);
        ApplySensitivity(sensitivitySlider.value);

        // Add listeners
        soundSlider.onValueChanged.AddListener(ApplySound);
        musicSlider.onValueChanged.AddListener(ApplyMusic);
        sensitivitySlider.onValueChanged.AddListener(ApplySensitivity);
    }

    private void ApplySound(float value)
    {
        // Adjust sound effect volume (usually via AudioMixer)
        // Example: AudioManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SoundVolume", value);
    }

    private void ApplyMusic(float value)
    {
        // Adjust background music volume (usually via AudioMixer)
        // Example: AudioManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void ApplySensitivity(float value)
    {
        // Apply player sensitivity (used in player controller)
        PlayerPrefs.SetFloat("Sensitivity", value);
        // Example: PlayerController.Instance.sensitivity = value;
    }
}
