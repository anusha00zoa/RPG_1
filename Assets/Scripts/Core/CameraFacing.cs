using UnityEngine;

namespace RPG.Core {
    public class CameraFacing : MonoBehaviour {

        Camera mainCamera;

        void Awake() {
            mainCamera = Camera.main;
        }

        // Update is called once per frame
        void LateUpdate() {
            transform.forward = mainCamera.transform.forward;
        }
    }
}
