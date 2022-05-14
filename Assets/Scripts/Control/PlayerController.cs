using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour {

        [System.Serializable]
        struct CursorMapping {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1.0f;
        [SerializeField] float raycastRadius = 1.0f;

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
            RaycastHit[] hits = RaycastAllSorted();

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
            Vector3 targetToMoveTo;
            bool hasHit = RaycastNavMesh(out targetToMoveTo);
            if (hasHit) {
                if(!GetComponent<Mover>().CanMoveTo(targetToMoveTo))
                    return false;

                // Input.GetMouseButton returns true as long as a mouse button is clicked
                // use it to allow the player to continuously follow the mouse
                if (Input.GetMouseButton(0)) {
                    // destination for the player's nav mesh agent
                    // player moves towards the point where the mouse clicked occured
                    //GetComponent<Mover>().StartMoveAction(hit.point, 1.0f);
                    GetComponent<Mover>().StartMoveAction(targetToMoveTo, 1.0f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target) {
            // check if the raycast has hit any unwalkable areas of the navmesh
            target = new Vector3();

            RaycastHit raycastHit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out raycastHit);
            if (!hasHit)
                return false;

            // find nearest navmesh point
            NavMeshHit navMeshHit;
            bool hasHitNavMesh = NavMesh.SamplePosition(raycastHit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasHitNavMesh) {
                return false;
            }
            target = navMeshHit.position;

            return true;
        }

        private void SetCursor(CursorType type) {
            // change cursor icon based om expected action by player
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type) {
            // get the correct icon based on the type of cursor
            foreach(CursorMapping mapping in cursorMappings)
                if (mapping.type == type)
                    return mapping;

            return cursorMappings[0];
        }

        RaycastHit[] RaycastAllSorted() {
            // sort all hits in raycasting based on distance
            RaycastHit[] raycastHits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            float[] distances = new float[raycastHits.Length];
            for (int i = 0; i < raycastHits.Length; i++) {
                distances[i] = raycastHits[i].distance;
            }
            Array.Sort(distances, raycastHits);

            return raycastHits;
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