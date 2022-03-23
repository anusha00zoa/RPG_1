using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicTrigger : MonoBehaviour {

        private bool alreadyTriggered = false;

        private void OnTriggerEnter(Collider other) {
            // trigger IntroSqeuence only once and get triggered only by Player
            if (!alreadyTriggered && other.gameObject.tag == "Player") {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            }
        }
    }
}
