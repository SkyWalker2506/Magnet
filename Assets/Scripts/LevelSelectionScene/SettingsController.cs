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
    
    public void OpenSettings()
    {
        this.gameObject.SetActive(true);
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
        if(value)
        {
            musicOnButton.gameObject.SetActive(false);
            musicOffButton.gameObject.SetActive(true);
            ToggleMusic();
        }
        else
        {
            musicOnButton.gameObject.SetActive(true);
            musicOffButton.gameObject.SetActive(false);
            ToggleMusic();
        }
    }
    public void SoundButtonChange(bool value)
    {
        if(value)
        {
            soundOnButton.gameObject.SetActive(false);
            soundOffButton.gameObject.SetActive(true);
            ToggleSfx();
        }
        else
        {
            soundOnButton.gameObject.SetActive(true);
            soundOffButton.gameObject.SetActive(false);
            ToggleSfx();
        }
    }
}
