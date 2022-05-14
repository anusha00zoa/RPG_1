using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using GameDevTV.Utils;

namespace RPG.Control {
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float suspicionTime = 5.0f;
        [SerializeField] float waypointDwellTime = 5.0f;
        [SerializeField] float waypointTolerance = 1.0f;
        [SerializeField] float aggroCoolDownTime = 5.0f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance;
        [SerializeField] PatrolPath patrolPath;

        LazyValue<Vector3> guardPosition;

        int currentWaypointIndex;

        float timeSinceLastSeenPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggravated = Mathf.Infinity;

        GameObject player;

        Fighter fighter;
        Health health;
        Mover mover;

        private void Awake() {
            // Find player using tags
            player = GameObject.FindWithTag("Player");

            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            guardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }

        private void Start() {
            // starting position for the character and the position it should return back to everytime (typically one of the waypoints on the patrol path)
            guardPosition.ForceInit();
        }

        private void Update() {
            if (health.IsDead())
                return;

            // check if player is within chase distance  or player has attacked you
            if (IsAggravated() && fighter.CanAttack(player)) {
                // reset and attack
                AttackBehaviour();
            }
            else if (timeSinceLastSeenPlayer < suspicionTime) {
                // enter suspicion state
                SuspicionBehaviour();
            }
            else {
                // cancel the attack
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggravate() {
            timeSinceAggravated = 0;
        }

        private Vector3 GetInitialGuardPosition() {
            return transform.position;
        }

        private void UpdateTimers() {
            timeSinceLastSeenPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private void SuspicionBehaviour() {
            // cancel all other actions
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour() {
            timeSinceLastSeenPlayer = 0.0f;
            // start attack
            fighter.Attack(player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies() {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);

            foreach(RaycastHit hit in hits) {
                AIController ai = hit.transform.GetComponent<AIController>();
                if(ai != null) {
                    hit.transform.GetComponent<AIController>().Aggravate();
                }
            }
        }

        private bool IsAggravated() {
            // checks if player is within chase distance of AI or if player has attacked this AI
            return (Vector3.Distance(transform.position, player.transform.position) < chaseDistance) || (timeSinceAggravated < aggroCoolDownTime);
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
            Vector3 nextPosition = guardPosition.value;

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