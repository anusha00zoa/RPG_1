using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;

namespace RPG.Attributes {
    public class Health : MonoBehaviour, ISaveable {

        [SerializeField] float healthPoints = -1f;
        [SerializeField] float regenerationPercent = 70.0f;

        private bool isDead = false;

        BaseStats baseStats;

        private void Start() {
            baseStats = GetComponent<BaseStats>();

            if (baseStats != null) {
                if (healthPoints < 0.0f) {
                    // RestoreState has not updated the healthPoints, we use basestats to get starting health
                    healthPoints = baseStats.GetStat(Stat.Health);
                }

                // subscribing to the delegate in BaseStats
                baseStats.onLevelUp += RegenerateHealth;
            }
        }

        // getter for isDead
        public bool IsDead() {
            return isDead;
        }

        // getter for max healthPoints
        public float GetMaxHealthPoints() {
            return baseStats.GetStat(Stat.Health);
        }

        // get current healthPoints
        public float GetHealthPoints() {
            return healthPoints;
        }

        // getter for healthPoints as a percentage
        public float GetHealthPercentage() {
            return healthPoints / baseStats.GetStat(Stat.Health) * 100;
        }

        public void TakeDamage(float damage, GameObject instigator) {
            //print(gameObject.name + " took damage: " + damage);

            healthPoints = Mathf.Max(healthPoints - damage, 0.0f);
            if (healthPoints == 0.0f) {
                Die();
                AwardExperience(instigator);
            }
        }

        void Die() {
            if (!isDead) {
                isDead = true;
                GetComponent<Animator>().SetTrigger("die");

                // when dead, the character should stop moving
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();

            if (experience == null)
                return;

            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth() {
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * regenerationPercent / 100;
            healthPoints = Mathf.Max(regenHealthPoints, healthPoints);
        }

        // member function required as we inherit from ISaveable
        public object CaptureState() {
            return healthPoints;
        }

        // member function required as we inherit from ISaveable
        public void RestoreState(object state) {
            healthPoints = (float)state;
            // if on restoration, the character is dead, we need to trigger death again
            if (healthPoints == 0.0f)
                Die();
        }
    }
}
