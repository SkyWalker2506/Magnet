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
    
    void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        instancedSystemPrefabs = new List<GameObject>();
        InstantiatingSystemPrefabs();
        LevelManager.Instance.LoadLevel((LevelManager.CurrentLevel).ToString());
    }
    
    private void OnEnable()
    {
        MagnetGameActionSystem.ObjectCollected += CheckIfLevelEnded;
    }
    private void OnDisable()
    {
        MagnetGameActionSystem.ObjectCollected -= CheckIfLevelEnded;
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        instancedSystemPrefabs.ForEach(Destroy);
        instancedSystemPrefabs.Clear();
    }

    void CheckIfLevelEnded(int collected)
    {
        if (collected == MagnetismManager.SceneMetals.Count)
        {
            MagnetGameActionSystem.OnLevelCompleted?.Invoke();
        }
    }
    
    

}