using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats {
    public class Experience : MonoBehaviour, ISaveable {

        [SerializeField] float experiencePoints = 0.0f;

        /* delegate for notifying game when user has levelled up
            first way to create your own delegate and create an event
            second way is to use C#'s built-in 'Action'
                - a predefined delegate with no return value and no parameters to pass in
        */
        // public delegate void ExperienceGainedDelegate();
        // public event ExperienceGainedDelegate OnExperienceGained;
        public event Action OnExperienceGained;

        public float GetPoints() {
            return experiencePoints;
        }

        public void GainExperience(float experience) {
            experiencePoints += experience;
        }

        // member function required as we inherit from ISaveable
        public object CaptureState() {
            return experiencePoints;
        }

        // member function required as we inherit from ISaveable
        public void RestoreState(object state) {
            experiencePoints = (float)state;
        }
    }
}
