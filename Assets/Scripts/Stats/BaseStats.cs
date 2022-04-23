using System;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {

        [Range(1, 5)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass = CharacterClass.Grunt;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        LazyValue<int> currentLevel;

        // delegate for notifying game when user has levelled up
        public event Action onLevelUp;

        Experience experience;

        private void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void OnEnable() {
            if (experience != null) {
                // subscribing to the delegate in Experience
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void Start() {
            currentLevel.ForceInit();
        }

        private void OnDisable() {
            if (experience != null) {
                // un-subscribing from the delegate in Experience
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        public float GetStat(Stat stat) {
            if (shouldUseModifiers)
                return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * GetPercentageModifier(stat);
            else
                return GetBaseStat(stat);
        }

        // getter for player level
        public int GetLevel() {
            return currentLevel.value;
        }

        private int CalculateLevel() {
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

        private float GetBaseStat(Stat stat) {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetAdditiveModifiers(Stat stat) {
            float total = 0.0f;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach(float modifier in provider.GetAdditiveModifiers(stat)) {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetPercentageModifier(Stat stat) {
            float result = 0.0f;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach(float modifier in provider.GetPercentageModifiers(stat)) {
                    result += modifier;
                }
            }

            return 1 + result / 100;
        }

        private void UpdateLevel() {
            // check if level has changed and update
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value) {
                currentLevel.value = newLevel;
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
