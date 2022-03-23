using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics {
    public class CinematicControlRemover : MonoBehaviour {
        // script to prevent the Player from playing th game while the IntroSequence is playing on screen
        // we use events to implement the observer pattern (as is available in c#)

        GameObject player;

        private void Start() {
            player = GameObject.FindWithTag("Player");

            // when IntroSequence starts playing, disable control for the player
            GetComponent<PlayableDirector>().played += DisableControl;
            // once IntroSequence is done playing, enable control for the player
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        void EnableControl(PlayableDirector pd) {
            // Debug.Log("EnableControl");

            // enable the PlayerController component
            player.GetComponent<PlayerController>().enabled = true;
        }

        void DisableControl(PlayableDirector pd) {
            // Debug.Log("DisableControl");

            // cancel any combat or movement we may have triggered on the player
            player.GetComponent<ActionScheduler>().CancelCurrentAction();

            // disable the PlayerController component
            player.GetComponent<PlayerController>().enabled = false;
        }
    }
}
