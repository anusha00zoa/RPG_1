using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat {
    public class Health : MonoBehaviour {

        [SerializeField] float health = 100.0f;

        public void TakeDamage(float damage) {
            health = Mathf.Max(health - damage, 0.0f);
            Debug.Log("health = " + health);
        }
    }
}
