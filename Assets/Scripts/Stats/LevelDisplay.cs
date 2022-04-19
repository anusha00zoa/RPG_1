using System;
using TMPro;
using UnityEngine;

namespace RPG.Stats {
    public class LevelDisplay : MonoBehaviour {

        BaseStats baseStats;

        TMP_Text levelValueText;

        private void Awake() {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            levelValueText = GetComponent<TMP_Text>();
        }

        void Update() {
            levelValueText.text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}
