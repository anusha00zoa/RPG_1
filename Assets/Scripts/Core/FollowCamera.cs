using UnityEngine;

namespace RPG.Core {

    public class FollowCamera : MonoBehaviour {

        [SerializeField]
        public GameObject target;

        void LateUpdate() {
            this.transform.position = target.transform.position;
        }
    }
}