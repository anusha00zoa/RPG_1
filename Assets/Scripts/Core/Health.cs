using UnityEngine;

namespace RPG.Core {
    public class Health : MonoBehaviour {

        [SerializeField] float healthPoints = 100.0f;

        private bool isDead = false;

        // getter for isDead
        public bool IsDead() {
            return isDead;
        }

        public void TakeDamage(float damage) {
            healthPoints = Mathf.Max(healthPoints - damage, 0.0f);
            Debug.Log("healthPoints = " + healthPoints);
            if (healthPoints == 0.0f)
                Die();
        }

        void Die() {
            if (!isDead) {
                isDead = true;
                GetComponent<Animator>().SetTrigger("die");

                // when dead, the character should stop moving
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }
    }
}
