using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider {

        [SerializeField] float timeBetweenAttacks = 1.0f;

        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] string defaultWeaponName = "Unarmed";

        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        float timeSinceLastAttack = Mathf.Infinity;

        Health target;

        Mover mover;

        private void Awake() {
            mover = GetComponent<Mover>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(GetDefaultWeapon);
        }

        private void Start() {
            currentWeapon.ForceInit();
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
            if (!GetIsInRange(target.transform)) {
                mover.MoveTo(target.transform.position, 1.0f);
            }
            else {
                // stop once in range of combat target
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void Attack(GameObject combatTarget) {
            GetComponent<ActionScheduler>().StartAction(this);

            //Debug.Log("Take that dipshit!!");
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget) {
            if (combatTarget == null)
                return false;

            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
                return false;

            // if there is no target and if the target is not dead, then we can attack
            if(!combatTarget.GetComponent<Health>().IsDead())
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

        private void StopAttack() {
            GetComponent<Animator>().SetTrigger("stopAttack");
            GetComponent<Animator>().ResetTrigger("attack");
        }

        public void EquipWeapon(WeaponConfig weapon) {
            if (weapon == null)
                return;

            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        public Health GetTarget() {
            return target;
        }

        private bool GetIsInRange(Transform targetTransform) {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetRange();
        }

        private Weapon GetDefaultWeapon() {
            return AttachWeapon(defaultWeapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon) {
            // spawn the weapon and override the animation with the appropriate weapon override
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        // method to implement ISaveable interface
        public object CaptureState() {
            return currentWeaponConfig.name;
        }

        // method to implement ISaveable interface
        public void RestoreState(object state) {
            string weaponName = (string)state;
            // allows us to load weapons across scenes by finding it in 'Weapons/Resources' folder
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        // method to implement IModifierProvider interface
        public IEnumerable<float> GetAdditiveModifiers(Stat stat) {
            if (stat == Stat.Damage) {
               yield return currentWeaponConfig.GetDamage();
            }
        }

        // method to implement IModifierProvider interface
        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if (stat == Stat.Damage) {
               yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        // begin animation event receivers
        void Hit() {
            // animation 'hit' event receiver

            // apply damage to combat target to match with the timing of the animation
            if (target == null)
                return;

            // get appropriate damage for the current level + equipped weapon
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            // check if weapon instance is not null
            if (currentWeapon.value !=  null) {
                currentWeapon.value.OnHit();
            }

            // if the equipped weapon has a projectile, launch it, if not take damage
            if (currentWeaponConfig.HasProjectile()) {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else {
                target.TakeDamage(damage, gameObject);
            }
        }

        void Shoot() {
            // animation 'shoot' event receiver
            // same as 'hit'
            Hit();
        }
        // end animation event receivers
    }
}