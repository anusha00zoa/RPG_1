using UnityEngine;
using TMPro;
using System;

namespace RPG.Attributes {
    public class HealthDisplay : MonoBehaviour {

        Health health;

        TMP_Text healthValueText;

        private void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            healthValueText = GetComponent<TMP_Text>();
        }

        void Update() {
            healthValueText.text = String.Format("{0:0}%", health.GetHealthPercentage());
        }
    }
}
