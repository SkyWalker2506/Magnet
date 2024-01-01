using UnityEngine;

namespace LevelSelection
{
    public class LevelSelectionView : MonoBehaviour
    {
        LevelModel[] levelData;
        [SerializeField] private LevelView levelViewPrefab;
        [SerializeField] private Transform levelViewHolder;

        public void CreateLevelViews(LevelModel[] levelModel)
        {
            levelData = levelModel;
            ClearLevelViewHolder();

            for (int i = 0; i < levelData.Length; i++)
            {
                var objPrefab = Instantiate(levelViewPrefab);
                objPrefab.transform.SetParent(levelViewHolder, false);

                objPrefab.SetLevel(levelData[i]);
            }
        }

        private void ClearLevelViewHolder()
        {
            if (levelViewHolder.transform.childCount == 0)
                return;

            for (int i = 0; i < levelViewHolder.childCount; i++)
            {
                Destroy(levelViewHolder.transform.GetChild(i).gameObject);
            }
        }
    }

}
