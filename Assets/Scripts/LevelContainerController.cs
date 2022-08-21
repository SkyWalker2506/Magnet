using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelContainerController : MonoBehaviour
{
    public int CurrentPage = 0;
    public int FirstPageBuildIndex = 3;
    public int LevelPerPage = 10;
    public int TotalLevel = 48;
    public GameObject LevelPrefab;
    public GameObject PagePrefab;
    List<GameObject> pages = new List<GameObject>();
    public Button PreviousPageButton;
    public Button NextPageButton;

    private void Awake()
    {
        SetPageLevels();
        SetCurrentLevelPage();
    }

    [ContextMenu("Set Pages")]
    void SetPages()
    {
        PreviousPageButton.interactable = CurrentPage!=0;
        NextPageButton.interactable = CurrentPage != pages.Count-1;
        pages.ForEach(p => p.SetActive(p == pages[CurrentPage]));
    }

    public void PrevPage()
    {
        if (CurrentPage >0)
        {
            CurrentPage--;
            SetPages();
        }
    }

    public void NextPage()
    {
        if (CurrentPage < pages.Count - 1)
        {
            CurrentPage++;
            SetPages();
        }
    }
    void SetCurrentLevelPage()
    {
        var currentLevel = PlayerPrefs.GetInt("gecilenLevel");
        CurrentPage = (currentLevel-1) / LevelPerPage;
        SetPages();
    }

    void SetPageLevels()
    {
        pages.ForEach(Destroy);
        pages.Clear();
        var totalPage = (TotalLevel / LevelPerPage)+1;
        for (int i = 0; i < totalPage; i++)
        {
            var page = Instantiate(PagePrefab,transform);
            page.name = "Page" + (i + 1);
            pages.Add(page);

            for (int j = LevelPerPage*i; j < Mathf.Min(LevelPerPage*(i+1),TotalLevel); j++)
            {
                var levelButton = Instantiate(LevelPrefab, page.transform).GetComponent<LevelButton>();
                levelButton.Level = j+1;
                levelButton.LevelBuildIndex = FirstPageBuildIndex + j;
                levelButton.SetLock();
            }
        }
    }
}