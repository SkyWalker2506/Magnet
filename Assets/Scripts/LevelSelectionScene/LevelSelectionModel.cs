using UnityEngine;

namespace LevelSelection
{
    [CreateAssetMenu(fileName = "LevelSelectionData")]
    public class LevelSelectionModel : ScriptableObject
    {
        public int SelectedLevel;
        public LevelModel[] LevelDatas;

        public void LoadData()
        {
            foreach (var levelData in LevelDatas) 
            {
                levelData.LoadData();
            }
        }
    }

}
