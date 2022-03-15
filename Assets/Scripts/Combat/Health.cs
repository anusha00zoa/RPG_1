using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat {
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
                GetComponent<Animator>().SetTrigger("die");
                isDead = true;
            }
        }
    }
}
