using UnityEngine;
using RPG.Saving;
using System;
using System.Collections;

namespace RPG.SceneManagement {
    public class SavingWrapper : MonoBehaviour {

        const string defaultSaveFile = "RPG_1_save";
        [SerializeField] float fadeInTime = 0.25f;

        // Start can return IEnumerator, automatically becomes a coroutine
        IEnumerator Start() {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);
        }

        void Update() {
            // if "L" is pressed on the keyboard, load the saved data from file
            if (Input.GetKeyDown(KeyCode.L)) {
                Load();
            }

            // if "S" is pressed on the keyboard, save data to file
            if(Input.GetKeyDown(KeyCode.S)) {
                Save();
            }
        }

        public void Save() {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load() {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
    }
}
