using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
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
        UnLoadLevel(CurrentLevel.ToString());
            CurrentLevel++;
        if (CurrentLevel >= maxLevel)
            CurrentLevel = 1;
        LoadLevel(CurrentLevel.ToString());
    }

    public void RestartLevel()
    {
        UnLoadLevel(CurrentLevel.ToString());
        LoadLevel(CurrentLevel.ToString());
    }

    public void LoadLevel(string levelName)
    {
        var ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return;
        }
        OnLoadOperationComplete(ao);
    }
    public void UnLoadLevel(string levelName)
    {
        var ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level " + levelName);
            return;
        }
        OnUnloadOperationComplete(ao);
    }
    
    void OnLoadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Load Complete.");
        MagnetGameActionSystem.LevelStarted?.Invoke(LevelManager.CurrentLevel);
    }
    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload Complete.");
    }

    void SetLevelCollectableAmount()
    {
        LevelCollectableAmount = Metal.SceneMetals.Count;
    }

    [Button("Set Level")]
    void SetLevel(int level)
    {
        CurrentLevel = level;
    }
    
}