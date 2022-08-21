using System;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]int levelToSet;
    [SerializeField] int maxLevel=15;

    public static int LevelCollectableAmount;
    public static float LevelPassTime=4;
    public static int CurrentLevel
    {
        get { return PlayerPrefs.GetInt("LastPassedLevel", 1); }
        set { PlayerPrefs.SetInt("LastPassedLevel", value); }
    }


    private void OnEnable()
    {
        MagnetGameActionSystem.ObjectCollected += CheckIfLevelEnded;
        MagnetGameActionSystem.LevelStarted += (l) => { SetLevelCollectableAmount(); };
    }
    private void OnDisable()
    {
        MagnetGameActionSystem.ObjectCollected -= CheckIfLevelEnded;
        MagnetGameActionSystem.LevelStarted -= (l) => { SetLevelCollectableAmount(); };
    }

    void CheckIfLevelEnded(int collected)
    {
        if (collected == Metal.SceneMetals.Count)
        {
            MagnetGameActionSystem.OnLevelCompleted?.Invoke();
            Invoke("OpenNextLevel", LevelPassTime);
        }
    }

    public void OpenNextLevel()
    {
        GameManager.Instance.UnLoadLevel(CurrentLevel.ToString());
            CurrentLevel++;
        if (CurrentLevel >= maxLevel)
            CurrentLevel = 1;
        GameManager.Instance.LoadLevel(CurrentLevel.ToString());
    }

    public void RestartLevel()
    {
        GameManager.Instance.UnLoadLevel(CurrentLevel.ToString());
        GameManager.Instance.LoadLevel(CurrentLevel.ToString());
    }


    void SetLevelCollectableAmount()
    {
        LevelCollectableAmount = Metal.SceneMetals.Count;
    }

    [ContextMenu("Set Level")]
    void SetLevel()
    {
        CurrentLevel = levelToSet;
    }
}