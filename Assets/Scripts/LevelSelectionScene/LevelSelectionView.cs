using UnityEngine;
using UnityEngine.UI;

namespace LevelSelection
{
    public class LevelSelectionView : MonoBehaviour
    {
        LevelModel[] levelData;
        [SerializeField] private LevelView levelViewPrefab;
        [SerializeField] private ScrollRect scrollRect; 
        ScrollStepLogic scrollStepLogic;

       public void UpdateScrollLogic(ref int selectedLevel)
        {
            if (scrollStepLogic == null)
            {
                return;
            }
            scrollStepLogic.Update(ref selectedLevel);
        }
        public void CreateScrollStepLogic()
        {
            scrollStepLogic = new ScrollStepLogic(scrollRect, true);
        }
        public void CreateLevelViews(LevelModel[] levelModel)
        {
            levelData = levelModel;
            ClearLevelViewHolder();

            for (int i = 0; i < levelData.Length; i++)
            {
                var objPrefab = Instantiate(levelViewPrefab);
                objPrefab.transform.SetParent(scrollRect.content, false);

                objPrefab.SetLevel(levelData[i]);
            }
        }

        private void ClearLevelViewHolder()
        {
            if (scrollRect.content.childCount == 0)
                return;

            for (int i = 0; i < scrollRect.content.childCount; i++)
            {
                Destroy(scrollRect.content.GetChild(i).gameObject);
            }
        }
    }

}
