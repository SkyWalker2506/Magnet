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
            levelSelectionModel.LoadData();
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
        }
        public void OpenLevel()
        {
            Debug.Log(levelSelectionModel.SelectedLevel);
            LevelManager.CurrentLevel = levelSelectionModel.SelectedLevel;
            SceneManager.LoadScene(1);

        }
    }

}
