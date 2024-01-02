using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text timeText;
    public Slider LevelProgressBar;
    public Animator LevelInOut;
    public GameObject GameUI;
    public GameObject FailedUI;

    void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += SetLevel;
        TimeManager.OnGameCountDownChanged += SetTime;
        MagnetGameActionSystem.ObjectCollected += SetProgressBar;
        MagnetGameActionSystem.OnLevelCompleted += LevelCompleted;
        MagnetGameActionSystem.OnLevelFailed += LevelFailed;
    }

    void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= SetLevel;
        TimeManager.OnGameCountDownChanged -= SetTime;
        MagnetGameActionSystem.ObjectCollected -= SetProgressBar;
        MagnetGameActionSystem.OnLevelCompleted -= LevelCompleted;
        MagnetGameActionSystem.OnLevelFailed -= LevelFailed;
    }

    private void Start()
    {
        SetProgressBar(0);
    }

    private void SetLevel(int level)
    {
        LevelInOut.SetTrigger("LevelIn");
        GameUI.SetActive(true);
        FailedUI.SetActive(false);
        levelText.text = level.ToString();
        SetProgressBar(0);
    }

    private void SetTime(int time)
    {
        timeText.SetText(time.ToString());
    }
    
    private void SetProgressBar(int collected)
    {
        LevelProgressBar.value = (float)collected / Mathf.Max(1,MagnetismManager.Instance.SceneMetals.Count);
    }

    void LevelCompleted()
    {
        GameUI.SetActive(false);
        LevelInOut.SetTrigger("LevelOut");
    }
    public void RestartLevel()
    {
        LevelManager.Instance.RestartLevel();
    }
    public void ReturnHome()
    {
        LevelManager.Instance.OpenHome();
    }

    void LevelFailed()
    {
        GameUI.SetActive(false);
        FailedUI.SetActive(true);
    }
}
