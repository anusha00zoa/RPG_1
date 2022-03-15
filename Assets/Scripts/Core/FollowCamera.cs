using System.Collections;
using System.Collections.Generic;
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