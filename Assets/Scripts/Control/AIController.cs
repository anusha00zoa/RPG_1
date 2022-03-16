using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control {
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        public float chaseDistance = 5.0f;

        GameObject player;

        Fighter fighter;
        Health health;

        private void Start() {
            // Find player using tags
            player = GameObject.FindWithTag("Player");

            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        private void Update() {
            if (health.IsDead())
                return;

            // check if player is within chase distance
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player)) {
                Debug.Log(this.gameObject.name + " should start chasing " + player.gameObject.name);

                // start attack
                fighter.Attack(player);
            }
            else {
                // cancel the attack
                fighter.Cancel();
            }
        }

        private bool InAttackRangeOfPlayer() {
            return Vector3.Distance(this.transform.position, player.transform.position) < chaseDistance;
        }
    }
}