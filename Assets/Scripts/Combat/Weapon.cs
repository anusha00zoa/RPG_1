using UnityEngine;
using RPG.Attributes;
using System;

namespace RPG.Combat {

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject {

        const string weaponName = "Weapon";

        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Projectile projectile = null;
        [SerializeField] float weaponRange = 1.0f;
        [SerializeField] float weaponDamage = 1.0f;
        [SerializeField] bool isRightHanded = true;

        // begin getter methods
        public float GetRange() {
            return weaponRange;
        }

        public float GetDamage() {
            return weaponDamage;
        }

        public bool HasProjectile() {
            return projectile != null;
        }
        // end getter methods

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage) {
            Projectile projectileInstance = Instantiate(projectile, GetHand(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator) {
            DestroyOldWeapon(rightHand, leftHand);

            // spawn the appropriate weapon in the correct hand
            if (equippedPrefab != null) {
                Transform handTransform = GetHand(rightHand, leftHand);
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            // override the animation with the appropriate weapon override
            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null) {
                // if we switch weapons and dont have a new override controller, we need to get the base 'character' controller and assign it again
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand) {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null) {
                // no weapon found in right hand, check in left hand
                oldWeapon = leftHand.Find(weaponName);
            }
            // if no weapon found in either hand, nothing to be destroyed
            if (oldWeapon == null)
                return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHand(Transform rightHand, Transform leftHand) {
            Transform handTransform;
            if (isRightHanded)
                handTransform = rightHand;
            else
                handTransform = leftHand;

            return handTransform;
        }
    }
}
