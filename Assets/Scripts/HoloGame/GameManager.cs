using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    string currentLevelName = string.Empty;
    [SerializeField]
    List<GameObject> systemPrefabs;
    List<GameObject> instancedSystemPrefabs;
    List<AsyncOperation> loadOperations;
    void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        instancedSystemPrefabs = new List<GameObject>();
        loadOperations = new List<AsyncOperation>();
        InstantiatingSystemPrefabs();
        //LoadLevel("MainScene");
        LoadLevel((LevelManager.CurrentLevel).ToString());
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

    void InstantiatingSystemPrefabs ()
    {
        GameObject prefabInstance;
        for (int i = 0; i < systemPrefabs.Count; i++)
        {
            prefabInstance=Instantiate(systemPrefabs[i]);
            prefabInstance.name = systemPrefabs[i].name;
            instancedSystemPrefabs.Add(prefabInstance);
        }
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        instancedSystemPrefabs.ForEach(Destroy);
        instancedSystemPrefabs.Clear();
    }

    

}