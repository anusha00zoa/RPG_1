using UnityEngine;

namespace RPG.Combat {

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject {

        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] float weaponRange = 2.0f;
        [SerializeField] float weaponDamage = 5.0f;

        public float GetRange() {
            return weaponRange;
        }

        public float GetDamage() {
            return weaponDamage;
        }

        public void Spawn(Transform handTransform, Animator animator) {
            // spawn the appropriate weapon
            if (equippedPrefab != null) {
                Instantiate(equippedPrefab, handTransform);
            }

            // override the animation with the appropriate weapon override
            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            }
        }
    }
}
