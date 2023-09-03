using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] TMP_Text levelText;
    public Image LevelProgressBar;
    public Animator LevelInOut;
    public GameObject GameUI;
    public GameObject CompletedUI;
    public GameObject FailedUI;

    void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += SetLevel;
        MagnetGameActionSystem.ObjectCollected += SetProgressBar;
        MagnetGameActionSystem.OnLevelCompleted += LevelCompleted;
        MagnetGameActionSystem.OnLevelFailed += LevelFailed;
        SetProgressBar(0);
    }

    void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= SetLevel;
        MagnetGameActionSystem.ObjectCollected -= SetProgressBar;
        MagnetGameActionSystem.OnLevelCompleted -= LevelCompleted;
        MagnetGameActionSystem.OnLevelFailed -= LevelFailed;
    }

    public void SetLevel(int level)
    {
        LevelInOut.SetTrigger("LevelIn");
        GameUI.SetActive(true);
        CompletedUI.SetActive(false);
        FailedUI.SetActive(false);
        levelText.text = level.ToString();
        SetProgressBar(0);
    }

    private void SetProgressBar(int collected)
    {
        LevelProgressBar.fillAmount = (float)collected / Metal.SceneMetals.Count;
    }

    void LevelCompleted()
    {
        GameUI.SetActive(false);
        CompletedUI.SetActive(true);
        LevelInOut.SetTrigger("LevelOut");
    }
    public void RestartLevel()
    {
        LevelManager.Instance.RestartLevel();
      //  SetLevel(LevelManager.CurrentLevel);
    }

    void LevelFailed()
    {
        GameUI.SetActive(false);
        FailedUI.SetActive(true);
    }
}
