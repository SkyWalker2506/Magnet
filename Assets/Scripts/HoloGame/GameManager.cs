using LevelSelection;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] List<GameObject> systemPrefabs;
    List<GameObject> instancedSystemPrefabs = new List<GameObject>();
    
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

    protected void OnDestroy()
    {
        instancedSystemPrefabs.ForEach(Destroy);
        instancedSystemPrefabs.Clear();
    }

    void CheckIfLevelEnded(int collected)
    {
        if (collected == MagnetismManager.Instance.SceneMetals.Count)
        {
            MagnetGameActionSystem.OnLevelCompleted?.Invoke();

            float endTime = TimeManager.Instance.LeftTime;
            int currentLevel = LevelManager.CurrentLevel-1;

            if (endTime >= 40)
                LevelSelectionView.Instance.levelData[currentLevel].StarCount = 3;
            if (endTime >= 20 && endTime <= 39)
                LevelSelectionView.Instance.levelData[currentLevel].StarCount = 2;
            if (endTime >= 1 && endTime <= 19)
                LevelSelectionView.Instance.levelData[currentLevel].StarCount = 1;

        }
    }
    
    

}