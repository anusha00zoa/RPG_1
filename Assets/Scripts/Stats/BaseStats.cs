using System;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {

        [Range(1, 5)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect = null;

        int currentLevel = 0;

        // delegate for notifying game when user has levelled up
        public event Action onLevelUp;

        private void Start() {
            currentLevel = CalculateLevel();
            Experience experience = GetComponent<Experience>();
            if (experience != null) {
                // subscribing to the delegate in Experience
                experience.onExperienceGained += UpdateLevel;
            }
        }

        public float GetStat(Stat stat) {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        // getter for player level
        public int GetLevel() {
            if (currentLevel < 1)
                currentLevel = CalculateLevel();

            return currentLevel;
        }

        public int CalculateLevel() {
            Experience experience = GetComponent<Experience>();
            if (experience == null)
                return startingLevel;

            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for (int i = 1; i <= penultimateLevel; i++) {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, i);
                if (currentXP <= XPToLevelUp) {
                    return i;
                }
            }

            // if our current XP is beyond all given levels info, return max level
            return penultimateLevel + 1;
        }

        private void UpdateLevel() {
            // check if level has changed and update
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel) {
                currentLevel = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect() {
            // play particle effect for levelling up
            Instantiate(levelUpEffect, transform);
        }
    }
}
