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

    /// <summary>Set by level selection before loading BootScene; Boot loads the level only when this is true.</summary>
    public static bool ShouldLoadLevelOnBoot { get; set; }


    private void OnEnable()
    {
        MagnetGameActionSystem.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(OpenNextLevel));
        MagnetGameActionSystem.OnLevelCompleted -= OnLevelCompleted;
    }

    private void OnLevelCompleted()
    {
        Invoke(nameof(OpenNextLevel), LevelPassTime);
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
        CancelInvoke(nameof(OpenNextLevel));
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
        CancelInvoke(nameof(OpenNextLevel));
        ShouldLoadLevelOnBoot = false;

        if (UIManager.IsInitialized)
            UIManager.Instance.HideForMenu();

        MagnetGameActionSystem.LevelUnloadedStarted?.Invoke();
        SceneManager.LoadScene(0);
    }

    protected override void OnDestroy()
    {
        CancelInvoke(nameof(OpenNextLevel));
        MagnetGameActionSystem.OnLevelCompleted -= OnLevelCompleted;
        base.OnDestroy();
    }
}