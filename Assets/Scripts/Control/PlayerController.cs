using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour {

        Health health;

        private void Start() {
            health = GetComponent<Health>();
        }

        void Update() {
        if (health.IsDead())
            return;

            // do player combat actions, dont move player while in combat
            if (InteractWithCombat())
                return;

            // if no combat occured, do player movement actions
            if (InteractWithMovement())
                return;
        }

        private bool InteractWithCombat() {
            // returns a list of all the hit results
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach(RaycastHit hit in hits) {
                // check if the hit object is a worthy combat component
                if (hit.transform.TryGetComponent(out CombatTarget ct)) {
                    // check if target is valid and alive and can be attacked
                    if (GetComponent<Fighter>().CanAttack(ct.gameObject)) {
                        // attack on mouse click
                        if (Input.GetMouseButton(0)) {
                            GetComponent<Fighter>().Attack(ct.gameObject);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool InteractWithMovement() {
            RaycastHit hit;

            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit) {
                // Input.GetMouseButton returns true as long as a mouse button is clicked
                // use it to allow the player to continuously follow the mouse
                if (Input.GetMouseButton(0)) {
                    // destination for the player's nav mesh agent
                    // player moves towards the point where the mouse clicked occured
                    GetComponent<Mover>().StartMoveAction(hit.point, 1.0f);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay() {
            // //ray from camera origin to point where mouse has clicked
            // //this ray will help us find points of intersection in our scene
            // //to see the line from the camera to the mouse click position
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Debug.DrawRay(ray.origin, ray.direction * 100, Color.white);
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}