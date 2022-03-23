using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement {
    public class Mover : MonoBehaviour, IAction {

        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6.0f;

        private NavMeshAgent navMeshAgent;
        Health health;

        private void Start() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update() {
            // to prevent a dead character's nav mesh agent blocking others from moving around/through it
            navMeshAgent.enabled = !health.IsDead();

            // animate our player character
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction) {
            GetComponent<ActionScheduler>().StartAction(this);
            // start movement
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 dest, float speedFraction) {
            navMeshAgent.destination = dest;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel() {
            // member function required as we inherit from IAction
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator() {
            // get global velocity from nav mesh agent
            Vector3 velocity = navMeshAgent.velocity;

            // convert this into a local value relative to the character
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            // we only care about the z which is forward for the character
            float speed = localVelocity.z;

            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }
    }
}
