using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{

    public int Level = 0;
    public int LevelBuildIndex = 3;

    public Text LevelText;
    public int currentLevel = 0;
    public GameObject Lock;


    private void Start()
    {
        SetLock();
    }
    private void OnEnable()
    {
        SetLock();
    }

    public void SetLock()
    {
        currentLevel = PlayerPrefs.GetInt("gecilenLevel");
        LevelText.text = Level.ToString();
        Lock.SetActive(Level > currentLevel);
    }

    public void OpenLevel()
    {
        SceneManager.LoadSceneAsync(LevelBuildIndex);
    }
}
