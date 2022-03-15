using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour, IAction {

        [SerializeField]
        float weaponRange = 2.0f;
        [SerializeField]
        float weaponDamage = 5.0f;

        [SerializeField]
        float timeBetweenAttacks = 1.0f;

        float timeSinceLastAttack = 0.0f;

        Transform target;

        Mover mover;

        private void Start() {
            mover = GetComponent<Mover>();
        }

        private void Update() {
            timeSinceLastAttack += Time.deltaTime;

            // if there is no combat target, then return from this function
            // allows the player to keep moving if we're just moving on the map
            if (target == null)
                return;

            // get within range of the combat target
            if (!GetIsInRange()) {
                mover.MoveTo(target.position);
            }
            else {
                // stop once in range of combat target
                mover.Cancel();
                AttackBehaviour();
            }
        }

        private bool GetIsInRange() {
            return Vector3.Distance(this.transform.position, target.position) < weaponRange;
        }

        private void AttackBehaviour() {
            // throttle the behaviour
            if (timeSinceLastAttack >= timeBetweenAttacks) {
                // trigger the animation, which will trigger the Hit() event for the animation
                GetComponent<Animator>().SetTrigger("attack");
                // reset
                timeSinceLastAttack = 0.0f;
            }
        }

        void Hit() {
            // animation 'hit' event receiver

            // apply damage to combat target to match with the timing of the animation
            Health healthComponent = target.GetComponent<Health>();
            healthComponent.TakeDamage(weaponDamage);
        }

        public void Attack (CombatTarget combatTarget) {
            GetComponent<ActionScheduler>().StartAction(this);

            Debug.Log("Take that dipshit!!");
            target = combatTarget.transform;
        }

        public void Cancel() {
            // member function required as we inherit from IAction
            target = null;
        }


    }
}