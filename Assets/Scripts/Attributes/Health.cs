using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes {
    public class Health : MonoBehaviour, ISaveable {

        [SerializeField] LazyValue<float> healthPoints;
        [SerializeField] float regenerationPercent = 70.0f;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> {}
        [SerializeField] TakeDamageEvent takeDamage;

        [SerializeField] UnityEvent onDie;

        private bool isDead = false;

        BaseStats baseStats;

        private void Awake() {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHeath);
        }

        private void OnEnable() {
            if (baseStats != null) {
                // subscribing to the delegate in BaseStats
                baseStats.onLevelUp += RegenerateHealth;
            }
        }

        private void Start() {
            healthPoints.ForceInit();
        }

        private void OnDisable() {
            if (baseStats != null) {
                // un-subscribing to the delegate in BaseStats
                baseStats.onLevelUp -= RegenerateHealth;
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
            return healthPoints.value;
        }

        public float GetHealthFraction() {
            return healthPoints.value / baseStats.GetStat(Stat.Health);
        }

        // getter for healthPoints as a percentage
        public float GetHealthPercentage() {
            return GetHealthFraction() * 100;
        }

        public void TakeDamage(float damage, GameObject instigator) {
            // update health based on damage
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0.0f);

            if (healthPoints.value == 0.0f) {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else {
                // trigger all the functions hooked up to this event
                takeDamage.Invoke(damage);
            }
        }

        public void Heal(float healthToRestore) {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        void Die() {
            if (!isDead) {
                isDead = true;
                GetComponent<Animator>().SetTrigger("die");

                // when dead, the character should stop moving
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }

        private float GetInitialHeath() {
            // RestoreState has not updated the healthPoints, we use basestats to get starting health
            return baseStats.GetStat(Stat.Health);
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();

            if (experience == null)
                return;

            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth() {
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * regenerationPercent / 100;
            healthPoints.value = Mathf.Max(regenHealthPoints, healthPoints.value);
        }

        // member function required as we inherit from ISaveable
        public object CaptureState() {
            return healthPoints.value;
        }

        // member function required as we inherit from ISaveable
        public void RestoreState(object state) {
            healthPoints.value = (float)state;
            // if on restoration, the character is dead, we need to trigger death again
            if (healthPoints.value == 0.0f)
                Die();
        }
    }
}
