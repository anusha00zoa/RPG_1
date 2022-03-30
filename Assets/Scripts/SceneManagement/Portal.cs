using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

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
        [SerializeField] float fadeOutTime = 2.0f;
        [SerializeField] float fadeInTime = 3.0f;
        [SerializeField] float fadeWaitTime = 0.5f;


        GameObject player;

        private bool alreadyTriggered = false;

        private void OnTriggerEnter(Collider other) {
            // trigger only by Player omce
            if (!alreadyTriggered && other.gameObject.tag == "Player") {
                StartCoroutine(Transition());
                alreadyTriggered = true;
            }
        }

        private IEnumerator Transition() {
            if(sceneToLoad < 0) {
                Debug.LogError("Scene To Load is not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone) {
                yield return null;
            }
            // Debug.Log("Scene finished loading.");

            // get portal in scene you just spawned player into and position player at spawn point
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(3.0f);

            // Destroy the portal after transitioning to prevent re-entry
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal) {
            // update player's position and rotation
            // use navmesh agent to update because sometimes it wants to set the player's position
            player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal() {
            // get the portal of the new scene we just loaded

            // foreach (Portal portal in FindObjectsOfType<Portal>()) {
            foreach (GameObject portal in GameObject.FindGameObjectsWithTag("Portal")) {
                if (portal == gameObject || portal == null)
                    continue;
                if(portal.GetComponent<Portal>().destination == destination)
                    continue;

                return portal.GetComponent<Portal>();
            }

            return null;
        }
    }
}