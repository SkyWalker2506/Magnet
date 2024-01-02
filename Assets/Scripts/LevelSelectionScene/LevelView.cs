using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelection
{
    public class LevelView : MonoBehaviour
    {
        public LevelModel levelData;
        [SerializeField] private Image levelImage;
        [SerializeField] private Image[] starImages;
        [SerializeField] private TMP_Text levelInfoText;

        [SerializeField] private GameObject lockedPanel;
        [SerializeField] private GameObject unlockedPanel;
        public void SetLevel(LevelModel data)
        {
            levelData = data;
            levelImage.sprite = data.LevelSprite;
            for (int i = 0; i < starImages.Length; i++)
            {
                starImages[i].DOFade(0, 0);
            }
            
            levelInfoText.SetText(data.IsUnlocked ? data.Level.ToString() : "LOCKED");
            lockedPanel.SetActive(data.IsUnlocked ? false : true);
            unlockedPanel.SetActive(data.IsUnlocked ? true : false);
        }

        public void OnFocus()
        {
            for (int i = 0; i < levelData.StarCount; i++)
            {
                if(LevelViewSwipe.Instance.selectedLevel == i)
                    starImages[i].DOFade(1, 1);
            }
        }
    }
}
