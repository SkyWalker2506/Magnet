using System;
using UnityEngine;

namespace LevelSelection
{
    [Serializable]
    public struct LevelModel
    {
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public Sprite LevelSprite { get; set; }

        public bool IsUnlocked => Level <= LevelManager.HighestUnlockedLevel;

        private string starsKey => $"Level {Level} Stars";

        public int StarCount
        {
            get => PlayerPrefs.GetInt(starsKey, 0);
            set
            {
                PlayerPrefs.SetInt(starsKey, value);
                PlayerPrefs.Save();
            }
        }
    }
}
