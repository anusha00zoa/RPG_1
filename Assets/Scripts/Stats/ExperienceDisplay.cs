using UnityEngine;
using TMPro;
using System;

namespace RPG.Stats {
    public class ExperienceDisplay : MonoBehaviour {

        Experience experience;

        TMP_Text XPValueText;

        private void Awake() {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            XPValueText = GetComponent<TMP_Text>();
        }

        void Update() {
            XPValueText.text = String.Format("{0:0} pts", experience.GetPoints());
        }
    }
}
