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
            levelSelectionView.CreateLevelViews(levelSelectionModel.LevelDatas);
        }

        public void OpenLevel()
        {
            
        }
    }

}
