using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour, IAction {

        [SerializeField]
        float weaponRange = 2.0f;

        Transform target;

        Mover mover;

        private void Start() {
            mover = GetComponent<Mover>();
        }

        private void Update() {
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
            // trigger the animation
            GetComponent<Animator>().SetTrigger("attack");
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

        void Hit() {
            // animation 'hit' event receiver
        }
    }
}