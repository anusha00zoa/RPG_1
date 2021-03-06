using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement {
    public class Portal : MonoBehaviour {

        enum DestinationIdentifier {
            Sandbox_destA,
            Sandbox_destB,
            Sandbox2_destA,
            Sandbox2_destB
        };

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1.0f;
        [SerializeField] float fadeInTime = 1.0f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private GameObject player;

        private void OnTriggerEnter(Collider other) {
            // trigger only by Player omce
            if (other.gameObject.tag == "Player") {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition() {
            if(sceneToLoad < 0) {
                Debug.LogError("Scene To Load is not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            // disable player control
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false;

            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);

            // Save current level
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone) {
                yield return null;
            }
            // Debug.Log("Scene finished loading.");

            // disable player control of new scene
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false;

            // Load current level
            savingWrapper.Load();

            // Get portal in scene you just spawned player into and position player at spawn point
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            // save at this point so that we can directly load at this point after we've gone through the portal
            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            // restore control
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = true;

            // Destroy the portal after transitioning to prevent re-entry
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal) {
            // Update player's position and rotation
            // Use navmesh agent to update because sometimes it wants to set the player's position
            player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal() {
            // get the portal of the new scene we just loaded

            // foreach (Portal portal in FindObjectsOfType<Portal>()) {
            foreach (GameObject portal in GameObject.FindGameObjectsWithTag("Portal")) {
                if (portal == gameObject)
                    continue;
                if(portal.GetComponent<Portal>().destination != destination)
                    continue;

                return portal.GetComponent<Portal>();
            }

            return null;
        }
    }
}