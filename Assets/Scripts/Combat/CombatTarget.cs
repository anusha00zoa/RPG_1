using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat {

    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable {

        // method to implement IRaycastable interface
        public bool HandleRaycast(PlayerController callingController) {

            // check if target is valid and alive and can be attacked
            if (GetComponent<Fighter>().CanAttack(gameObject)) {
                // attack this on mouse click
                if (Input.GetMouseButton(0)) {
                    callingController.GetComponent<Fighter>().Attack(gameObject);
                }
                return true;
            }

            return false;
        }

        // method to implement IRaycastable interface
        public CursorType GetCursorType() {
            return CursorType.Combat;
        }
    }
}
