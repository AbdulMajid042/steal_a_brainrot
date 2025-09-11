using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource musicAudioSource;
    public AudioSource clickAudioSource;
    public AudioSource genericAudioSource;
    public AudioClip negativeSfx, positiveSfx;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Update()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume= PlayerPrefs.GetFloat("MusicVolume");
        }
        if (clickAudioSource != null)
        {
            clickAudioSource.volume = PlayerPrefs.GetFloat("SoundVolume");
        }
    }
    public void PlayClickSound()
    {
        clickAudioSource.PlayOneShot(clickAudioSource.clip);
    }
    public void PlayGenericSound(AudioClip clip)
    {
        clickAudioSource.PlayOneShot(clip);
    }
}
