using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    [SerializeField] List<GameObject> systemPrefabs;
    List<GameObject> instancedSystemPrefabs = new List<GameObject>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            if (LevelManager.ShouldLoadLevelOnBoot && LevelManager.IsInitialized)
            {
                LevelManager.ShouldLoadLevelOnBoot = false;
                LevelManager.Instance.LoadLevel(LevelManager.CurrentLevel.ToString());
            }

            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (_instance != this)
            return;

        Application.targetFrameRate = 60;

        if (!LevelManager.IsInitialized)
            InstantiatingSystemPrefabs();

        if (LevelManager.ShouldLoadLevelOnBoot)
        {
            LevelManager.ShouldLoadLevelOnBoot = false;
            LevelManager.Instance.LoadLevel(LevelManager.CurrentLevel.ToString());
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SceneManager.LoadScene(0);
        }
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
            prefabInstance = Instantiate(systemPrefabs[i]);
            prefabInstance.name = systemPrefabs[i].name;
            DontDestroyOnLoad(prefabInstance);
            instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    void OnDestroy()
    {
        if (_instance != this)
            return;

        _instance = null;
        instancedSystemPrefabs.ForEach(Destroy);
        instancedSystemPrefabs.Clear();
    }

    void CheckIfLevelEnded(int collected)
    {
        if (!LevelManager.IsInitialized || MagnetismManager.Instance == null)
            return;

        if (collected == MagnetismManager.Instance.SceneMetals.Count)
        {
            MagnetGameActionSystem.OnLevelCompleted?.Invoke();
            AwardStars();
        }
    }

    static void AwardStars()
    {
        float endTime = TimeManager.Instance != null ? TimeManager.Instance.LeftTime : 0f;
        int stars;
        if (endTime >= 40) stars = 3;
        else if (endTime >= 20) stars = 2;
        else if (endTime >= 1) stars = 1;
        else return;

        string key = $"Level {LevelManager.CurrentLevel} Stars";
        if (stars > PlayerPrefs.GetInt(key, 0))
        {
            PlayerPrefs.SetInt(key, stars);
            PlayerPrefs.Save();
        }
    }
    
    

}