using UnityEngine;

namespace RPG.Control {
    public class PatrolPath : MonoBehaviour {

        const float waypointGizmoRadius = 0.5f;

        private void OnDrawGizmos() {
            Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 0.25f);

            for (int i = 0; i < transform.childCount; i++) {
                Vector3 childPosition = GetWaypoint(i);

                // sphere to represent the current waypoint
                Gizmos.DrawSphere(childPosition, waypointGizmoRadius);

                // draw line between consecutive waypoints
                Gizmos.DrawLine(childPosition, GetWaypoint(GetNextIndex(i)));
            }
        }

        public Vector3 GetWaypoint(int i) {
            return transform.GetChild(i).position;
        }

        public int GetNextIndex(int i) {
            if (i + 1 == transform.childCount)
                return 0;

            return i + 1;
        }

    }
}
