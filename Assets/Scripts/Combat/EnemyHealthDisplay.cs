using UnityEngine;
using TMPro;
using System;
using RPG.Attributes;

namespace RPG.Combat {
    public class EnemyHealthDisplay : MonoBehaviour {

        Fighter fighter;
        Health target;

        TMP_Text healthValueText;

        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            healthValueText = GetComponent<TMP_Text>();
        }

        void Update() {
            target = fighter.GetTarget();
            if (target == null) {
                healthValueText.text = "No target";
                return;
            }
            healthValueText.text = String.Format("{0:0}%", target.GetHealthPercentage());
        }
    }
}
