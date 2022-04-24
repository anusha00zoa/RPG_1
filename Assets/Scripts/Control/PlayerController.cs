using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour {

        [System.Serializable]
        struct CursorMapping {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

        Health health;

        private void Awake(){
            health = GetComponent<Health>();
        }

        void Update() {
            // check if can interact with UI
            if (InteractWithUI())
                return;

            if (health.IsDead()) {
                SetCursor(CursorType.None);
                return;
            }

            // do player combat actions or pickup actions, dont move player while in combat
            if (InteractWithComponent())
                return;

            // if no combat occured, do player movement actions
            if (InteractWithMovement())
                return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent() {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach(RaycastHit hit in hits) {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();

                foreach(IRaycastable raycastable in raycastables) {
                    if(raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        private bool InteractWithUI() {
            // returns true if cursor is over UI and false if anywhere else
            if (EventSystem.current.IsPointerOverGameObject()) {
                SetCursor(CursorType.UI);
                return true;
            }

            return false;
        }

        private bool InteractWithMovement() {
            RaycastHit hit;

            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit) {
                // Input.GetMouseButton returns true as long as a mouse button is clicked
                // use it to allow the player to continuously follow the mouse
                if (Input.GetMouseButton(0)) {
                    // destination for the player's nav mesh agent
                    // player moves towards the point where the mouse clicked occured
                    GetComponent<Mover>().StartMoveAction(hit.point, 1.0f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private void SetCursor(CursorType type) {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type) {
            foreach(CursorMapping mapping in cursorMappings)
                if (mapping.type == type)
                    return mapping;

            return cursorMappings[0];
        }

        private static Ray GetMouseRay() {
            // //ray from camera origin to point where mouse has clicked
            // //this ray will help us find points of intersection in our scene
            // //to see the line from the camera to the mouse click position
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Debug.DrawRay(ray.origin, ray.direction * 100, Color.white);
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}