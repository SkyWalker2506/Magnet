using UnityEngine;

namespace LevelSelection
{
    public class LevelSelectionView : MonoBehaviour
    {
        [SerializeField] private LevelView levelViewPrefab;
        [SerializeField] private Transform levelViewHolder;

        public void CreateLevelViews(LevelModel[] levelModel)
        {
            
        }
    }

}
