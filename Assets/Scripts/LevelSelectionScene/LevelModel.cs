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
            get => SaveService.GetInt(starsKey, 0);
            set
            {
                SaveService.SetInt(starsKey, value);
                SaveService.Save();
            }
        }
    }
}
