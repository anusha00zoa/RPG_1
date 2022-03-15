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

        Health target;

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

            // if target is dead, dont do any attacking
            if (target.IsDead())
                return;

            // get within range of the combat target
            if (!GetIsInRange()) {
                mover.MoveTo(target.transform.position);
            }
            else {
                // stop once in range of combat target
                mover.Cancel();
                AttackBehaviour();
            }
        }

        private bool GetIsInRange() {
            return Vector3.Distance(this.transform.position, target.transform.position) < weaponRange;
        }

        private void AttackBehaviour() {
            // face the target
            transform.LookAt(target.transform);

            // throttle the behaviour
            if (timeSinceLastAttack >= timeBetweenAttacks) {
                // trigger the animation, which will trigger the Hit() event for the animation
                TriggerAttack();

                // reset
                timeSinceLastAttack = 0.0f;
            }
        }

        private void TriggerAttack() {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        void Hit() {
            // animation 'hit' event receiver

            // apply damage to combat target to match with the timing of the animation
            if (target == null)
                return;

            target.TakeDamage(weaponDamage);
            // Health healthComponent = target.GetComponent<Health>();
            // healthComponent.TakeDamage(weaponDamage);
        }

        public void Attack (CombatTarget combatTarget) {
            GetComponent<ActionScheduler>().StartAction(this);

            //Debug.Log("Take that dipshit!!");
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(CombatTarget ct) {
            // if there is no target and if the target is not dead, then we can attack
            if(ct != null && !ct.GetComponent<Health>().IsDead())
                return true;

            return false;
        }

        public void Cancel() {
            // stop the animation
            StopAttack();

            // member function required as we inherit from IAction
            target = null;
        }

        private void StopAttack() {
            GetComponent<Animator>().SetTrigger("stopAttack");
            GetComponent<Animator>().ResetTrigger("attack");
        }
    }
}