using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;

namespace RPG.Control {
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float suspicionTime = 5.0f;
        [SerializeField] float waypointDwellTime = 5.0f;
        [SerializeField] float waypointTolerance = 1.0f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] PatrolPath patrolPath;

        Vector3 guardPosition;

        int currentWaypointIndex = 0;

        float timeSinceLastSeenPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;

        GameObject player;

        Fighter fighter;
        Health health;
        Mover mover;

        private void Start() {
            // Find player using tags
            player = GameObject.FindWithTag("Player");

            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            // starting position for the character and the position it should return back to everytime (typically one of the waypoints on the patrol path)
            guardPosition = transform.position;
        }

        private void Update() {
            if (health.IsDead())
                return;

            // check if player is within chase distance
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player)) {
                // reset and attack
                AttackBehaviour();
            }
            else if (timeSinceLastSeenPlayer < suspicionTime) {
                // enter suspicion state
                SuspicionBehaviour();
            }
            else {
                // // cancel the attack
                // fighter.Cancel();
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers() {
            timeSinceLastSeenPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void SuspicionBehaviour() {
            // cancel all other actions
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour() {
            timeSinceLastSeenPlayer = 0.0f;
            // start attack
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer() {
            return Vector3.Distance(transform.position, player.transform.position) < chaseDistance;
        }

        private Vector3 GetCurrentWaypoint() {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private bool AtWaypoint() {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoints() {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private void PatrolBehaviour(){
            // start moving along patrol path
            Vector3 nextPosition = guardPosition;

            if (patrolPath != null) {
                if(AtWaypoint()) {
                    timeSinceArrivedAtWaypoint = 0.0f;
                    CycleWaypoints();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > waypointDwellTime) {
                // StartMoveAction should automatically cancel the attack
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private void OnDrawGizmosSelected() {
            // allows us to view gizmos in scene mode, called bu unity

            // color to use to draw the gizmos
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            Gizmos.DrawWireSphere(transform.position, chaseDistance);

        }
    }
}