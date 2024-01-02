using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] int maxLevel=15;

    public static readonly float LevelPassTime = 3.5f;
    public static int CurrentLevel
    {
        get { return PlayerPrefs.GetInt("LastPassedLevel", 1); }
        set { PlayerPrefs.SetInt("LastPassedLevel", value); }
    }


    private void OnEnable()
    {
        MagnetGameActionSystem.OnLevelCompleted += ()=>Invoke("OpenNextLevel", LevelPassTime);
    }
    private void OnDisable()
    {
        MagnetGameActionSystem.OnLevelCompleted -=  ()=>Invoke("OpenNextLevel", LevelPassTime);
    }

    public void OpenNextLevel()
    {
        UnLoadLevel(CurrentLevel.ToString());
            CurrentLevel++;
        if (CurrentLevel > maxLevel)
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
        MagnetGameActionSystem.LevelUnloadedStarted?.Invoke();
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
        MagnetGameActionSystem.LevelUnloadedEnded?.Invoke();
        Debug.Log("Unload Complete.");
    }

    [Button("Set Level")]
    void SetLevel(int level)
    {
        CurrentLevel = level;
    }

    public void OpenHome()
    {
        SceneManager.LoadScene(0);
    }
}