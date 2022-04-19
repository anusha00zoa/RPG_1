using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;

namespace RPG.Attributes {
    public class Health : MonoBehaviour, ISaveable {

        float healthPoints = -1f;

        private float maxHealthPoints = 0.0f;

        private bool isDead = false;

        private void Start() {
            if (healthPoints < 0.0f) {
                // RestoreState has not updated the healthPoints, we use basestats to get starting health
                healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);
                maxHealthPoints = healthPoints;
            }
        }

        // getter for isDead
        public bool IsDead() {
            return isDead;
        }

        // getter for healthPoints as a percentage
        public float GetHealthPercentage() {
            return healthPoints / maxHealthPoints * 100;
        }

        public void TakeDamage(float damage, GameObject instigator) {
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

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
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
