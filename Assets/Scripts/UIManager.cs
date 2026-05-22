using System;
using DG.Tweening;
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

    int collectedCount;
    int totalMetals;
    Tween progressTween;

    void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += SetLevel;
        TimeManager.OnGameCountDownChanged += SetTime;
        MagnetGameActionSystem.OnMetalCollected += OnMetalCollected;
        MagnetGameActionSystem.OnLevelCompleted += LevelCompleted;
        MagnetGameActionSystem.OnLevelFailed += LevelFailed;
    }

    void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= SetLevel;
        TimeManager.OnGameCountDownChanged -= SetTime;
        MagnetGameActionSystem.OnMetalCollected -= OnMetalCollected;
        MagnetGameActionSystem.OnLevelCompleted -= LevelCompleted;
        MagnetGameActionSystem.OnLevelFailed -= LevelFailed;
    }

    private void Start()
    {
        RefreshProgressBar();
    }

    private void SetLevel(int level)
    {
        gameObject.SetActive(true);
        LevelInOut.SetTrigger("LevelIn");
        SetGameUIVisible(true);
        FailedUI.SetActive(false);
        levelText.text = level.ToString();
        collectedCount = 0;
        totalMetals = MagnetismManager.Instance != null
            ? MagnetismManager.Instance.SceneMetals.Count
            : 0;
        RefreshProgressBar();
    }

    public void HideForMenu()
    {
        SetGameUIVisible(false);
        if (FailedUI != null)
            FailedUI.SetActive(false);
    }

    private void SetGameUIVisible(bool visible)
    {
        if (GameUI == null) return;
        var canvas = GameUI.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.enabled = visible;
            GameUI.SetActive(true);
        }
        else
        {
            GameUI.SetActive(visible);
        }
    }

    private void SetTime(int time)
    {
        timeText.SetText(time.ToString());
    }

    private void OnMetalCollected(Metal metal)
    {
        collectedCount++;
        RefreshProgressBar();
    }

    private void RefreshProgressBar()
    {
        float target = totalMetals > 0 ? (float)collectedCount / totalMetals : 0f;
        if (progressTween != null && progressTween.IsActive()) progressTween.Kill();
        progressTween = DOTween.To(() => LevelProgressBar.value, v => LevelProgressBar.value = v, target, 0.25f)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    void LevelCompleted()
    {
        LevelInOut.SetTrigger("LevelOut");
    }
    public void RestartLevel()
    {
        if (!LevelManager.IsInitialized)
            return;

        LevelManager.Instance.RestartLevel();
    }

    public void ReturnHome()
    {
        if (!LevelManager.IsInitialized)
            return;

        LevelManager.Instance.OpenHome();
    }

    void LevelFailed()
    {
        FailedUI.SetActive(true);
    }
}
