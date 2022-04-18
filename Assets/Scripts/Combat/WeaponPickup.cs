using System;
using System.Collections;
using UnityEngine;

namespace RPG.Combat {

    public class WeaponPickup : MonoBehaviour {

        [SerializeField] Weapon weapon = null;
        [SerializeField] float respawnTime = 5.0f;

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag == "Player") {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(ReSpawnInSeconds(respawnTime));
                //Destroy(gameObject);
            }
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
