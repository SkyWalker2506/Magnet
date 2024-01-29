using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Sound[] MusicSound, SfxSounds;
    public AudioSource MusicSource, SfxSource;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("GameMusic");
    }

    public void PlayMusic(string name)
    {
        Sound s = System.Array.Find(MusicSound, sound => sound.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
        {
            MusicSource.clip = s.Clip;
            MusicSource.Play();
        }
    }
    public void PlaySfx(string name)
    {
        Sound s = System.Array.Find(SfxSounds, x => x.Name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        else
        {
            SfxSource.PlayOneShot(s.Clip);
        }
    }

    public void ToggleMusic()
    {
        MusicSource.mute = !MusicSource.mute;
    }
    public void ToggleSfx()
    {
        SfxSource.mute = !SfxSource.mute;
    }
    public void MusicVolumeChange(float value)
    {
        MusicSource.volume = value;
    }
}
