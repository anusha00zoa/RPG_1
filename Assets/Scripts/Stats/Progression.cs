using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats {

    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject {

        [SerializeField] CharacterProgressionClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level) {
            BuildLookupTable();

            float[] levels = lookupTable[characterClass][stat];
            if (level > levels.Length) {
                return 0;
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass) {
            BuildLookupTable();
            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookupTable() {
            if (lookupTable != null)
                return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (CharacterProgressionClass pClass in characterClasses) {
                Dictionary<Stat, float[]> statLookup = new Dictionary<Stat, float[]>();

                foreach (StatProgressionClass pStat in pClass.stats) {
                    statLookup.Add(pStat.stat, pStat.levels);
                }
                lookupTable.Add(pClass.characterClass, statLookup);
            }
        }

        [System.Serializable]
        class CharacterProgressionClass {

            public CharacterClass characterClass;
            public StatProgressionClass[] stats;
        }

        [System.Serializable]
        class StatProgressionClass {

            public Stat stat;
            public float[] levels = null;
        }
    }
}