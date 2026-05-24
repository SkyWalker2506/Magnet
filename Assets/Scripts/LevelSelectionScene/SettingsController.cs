using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    
    [SerializeField] Button musicOnButton;
    [SerializeField] Button musicOffButton;
    [SerializeField] Button soundOnButton;
    [SerializeField] Button soundOffButton;

    [SerializeField] Slider musicVolumeSlider;

    
    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SyncButtonsFromSoundManager();
    }

    public void OpenSettings()
    {
        this.gameObject.SetActive(true);
    }

    void SyncButtonsFromSoundManager()
    {
        if (SoundManager.Instance == null) return;

        bool musicOn = SoundManager.Instance.MusicSource != null && !SoundManager.Instance.MusicSource.mute;
        if (musicOnButton != null) musicOnButton.gameObject.SetActive(musicOn);
        if (musicOffButton != null) musicOffButton.gameObject.SetActive(!musicOn);

        bool sfxOn = SoundManager.Instance.SfxSource != null && !SoundManager.Instance.SfxSource.mute;
        if (soundOnButton != null) soundOnButton.gameObject.SetActive(sfxOn);
        if (soundOffButton != null) soundOffButton.gameObject.SetActive(!sfxOn);

        if (musicVolumeSlider != null && SoundManager.Instance.MusicSource != null)
            musicVolumeSlider.SetValueWithoutNotify(SoundManager.Instance.MusicSource.volume);
    }

    public void CloseSettings()
    {
        this.gameObject.SetActive(false);
    }

    public void SaveSettings()
    {
        
        CloseSettings();
    }

    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMusic();
    }
    public void ToggleSfx()
    {
        SoundManager.Instance.ToggleSfx();
    }

    public void MusicVolumeChange()
    {
        SoundManager.Instance.MusicVolumeChange(musicVolumeSlider.value);
    }

    public void MusicButtonChange(bool value)
    {
        ToggleMusic();
        SyncButtonsFromSoundManager();
    }
    public void SoundButtonChange(bool value)
    {
        ToggleSfx();
        SyncButtonsFromSoundManager();
    }
}
