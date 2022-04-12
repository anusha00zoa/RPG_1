using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction {

        [SerializeField] float timeBetweenAttacks = 1.0f;
        [SerializeField] Transform handTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        Weapon currentWeapon = null;

        float timeSinceLastAttack = Mathf.Infinity;

        Health target;

        Mover mover;

        private void Start() {
            mover = GetComponent<Mover>();

            EquipWeapon(defaultWeapon);
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
                mover.MoveTo(target.transform.position, 1.0f);
            }
            else {
                // stop once in range of combat target
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void Attack (GameObject combatTarget) {
            GetComponent<ActionScheduler>().StartAction(this);

            //Debug.Log("Take that dipshit!!");
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget) {
            // if there is no target and if the target is not dead, then we can attack
            if(combatTarget != null && !combatTarget.GetComponent<Health>().IsDead())
                return true;

            return false;
        }

        public void Cancel() {
            // stop the animation
            StopAttack();

            // member function required as we inherit from IAction
            target = null;

            // cancel movement
            GetComponent<Mover>().Cancel();
        }

        public void EquipWeapon(Weapon weapon) {
            if (weapon == null)
                return;

            currentWeapon = weapon;

            // spawn the weapon and override the animation with the appropriate weapon override
            Animator animator = GetComponent<Animator>();
            currentWeapon.Spawn(handTransform, animator);
        }

        private bool GetIsInRange() {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange();
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

            target.TakeDamage(currentWeapon.GetDamage());
            // Health healthComponent = target.GetComponent<Health>();
            // healthComponent.TakeDamage(weaponDamage);
        }

        private void StopAttack() {
            GetComponent<Animator>().SetTrigger("stopAttack");
            GetComponent<Animator>().ResetTrigger("attack");
        }
    }
}