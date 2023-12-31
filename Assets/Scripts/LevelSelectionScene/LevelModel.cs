using System;
using UnityEngine;

namespace LevelSelection
{
    [Serializable]
    public struct LevelModel
    {
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public Sprite LevelSprite { get; set; }
        [field: SerializeField] public bool IsUnlocked { get; set; }
        [field: SerializeField] public int StarCount { get; set; }

        private string saveKey => $"Level {Level} Save Key";

        public void SaveData()
        {
            PlayerPrefs.SetString(saveKey, JsonUtility.ToJson(this));
        }
        public void LoadData()
        {
            if(PlayerPrefs.HasKey(saveKey))
            {
                var json = PlayerPrefs.GetString(saveKey);
                var data = JsonUtility.FromJson<LevelModel>(json);
                Level = data.Level;
                LevelSprite = data.LevelSprite;
                IsUnlocked = data.IsUnlocked;
                StarCount = data.StarCount;
            }

        }
    }
}
