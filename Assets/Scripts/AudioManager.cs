using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    static AudioSource audioSource;
    [SerializeField]
    AudioClip metalHitClip;
    [SerializeField]
    AudioClip dropToHoleClip;
    [SerializeField]
    AudioClip dropToLavaClip;
    void Start()
    {
        audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
    }

    static void PlayClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public static void PlayMetalHitClip()
    {
        PlayClip(Instance.metalHitClip);
    }
    public static void PlayDropToHoleClip()
    {
        PlayClip(Instance.dropToHoleClip);
    }
    public static void PlayDropToLavaClip()
    {
        PlayClip(Instance.dropToLavaClip);
    }
}
