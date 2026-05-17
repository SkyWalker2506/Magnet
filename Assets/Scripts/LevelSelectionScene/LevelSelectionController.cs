using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LevelSelection
{
    public class LevelSelectionController : MonoBehaviour
    {
        [SerializeField] private LevelSelectionView levelSelectionView;
        [SerializeField] private LevelSelectionModel levelSelectionModel;

        private void Awake()
        {
            levelSelectionModel.SelectedLevel = 0;
            levelSelectionView.CreateLevelViews(levelSelectionModel.LevelDatas);
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            levelSelectionView.CreateScrollStepLogic();

        }

        private void Update()
        {
            levelSelectionView.UpdateScrollLogic(ref levelSelectionModel.SelectedLevel);
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                OpenLevel();
        }
        public void OpenLevel()
        {
            int level = levelSelectionModel.SelectedLevel;
            if (level <= 0 || level > LevelManager.HighestUnlockedLevel) return;
            LevelManager.CurrentLevel = level;
            LevelManager.ShouldLoadLevelOnBoot = true;
            SceneManager.LoadScene(1);
        }
    }

}
