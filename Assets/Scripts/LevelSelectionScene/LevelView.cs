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
            OnUnfocus();

            levelInfoText.SetText(data.IsUnlocked ? data.Level.ToString() : "LOCKED");
            lockedPanel.SetActive(data.IsUnlocked ? false : true);
            unlockedPanel.SetActive(data.IsUnlocked ? true : false);
        }

        public void OnFocus()
        {
            
            AnimateStarAlphas();
            
        }

        public void OnUnfocus()
        {
            foreach (var starImage in starImages)
            {
                starImage.color = new Color(1, 1, 1, 0);
            }

        }

        public void AnimateStarAlphas()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(0.5f); // Wait for 1 second
            for (int i = 0; i < levelData.StarCount; i++)
            {
                sequence.Append(starImages[i].DOFade(1, 0.5f));
            }
            sequence.Play();
        }
    }
}
