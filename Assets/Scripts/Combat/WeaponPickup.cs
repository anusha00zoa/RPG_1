using System;
using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat {

    public class WeaponPickup : MonoBehaviour, IRaycastable {

        [SerializeField] Weapon weapon = null;
        [SerializeField] float respawnTime = 5.0f;

        // method to implement IRaycastable interface
        public bool HandleRaycast(PlayerController callingController) {
            if (Input.GetMouseButtonDown(0)) {
                Pickup(callingController.GetComponent<Fighter>());
            }

            // we should always allow a pickup to be picked up even if just hovering
            return true;
        }

        public CursorType GetCursorType() {
            return CursorType.Pickup;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag == "Player") {
                Pickup(other.GetComponent<Fighter>());
            }
        }

        private void Pickup(Fighter fighter) {
            fighter.EquipWeapon(weapon);
            StartCoroutine(ReSpawnInSeconds(respawnTime));
        }

        private IEnumerator ReSpawnInSeconds(float seconds) {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow) {
            // disable/re-enable the collider
            GetComponent<Collider>().enabled = shouldShow;
            // disable/re-enable the children
            foreach (Transform child in transform) {
                child.gameObject.SetActive(shouldShow);
            }
        }
    }
}
