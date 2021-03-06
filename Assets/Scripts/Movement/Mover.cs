using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement {
    public class Mover : MonoBehaviour, IAction, ISaveable {

        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6.0f;

        private NavMeshAgent navMeshAgent;
        Health health;

        [SerializeField] float maxNavPathLength = 40.0f;


        private void Awake() {
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

        public bool CanMoveTo(Vector3 destination) {
            // get path from player's position to clicked position
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath)
                return false;

            if (path.status != NavMeshPathStatus.PathComplete)
                return false;

            // we dont want paths that are extremely long or that run through enemies
            if (GetPathLength(path) > maxNavPathLength)
                return false;

            return true;
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

        private float GetPathLength(NavMeshPath path) {
            float pathLength = 0.0f;

            if(path.corners.Length < 2)
                return pathLength;

            for(int i = 0; i < path.corners.Length - 1; i++)
                pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);

            return pathLength;
        }

        // using a struct to capture / restore data - do not need to do any casting
        [System.Serializable]
        struct MoverSaveData {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        // member function required as we inherit from ISaveable
        public object CaptureState() {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        // member function required as we inherit from ISaveable
        public void RestoreState(object state) {
            // casting to MoverSaveData because we know for sure that is what it is stored as
            MoverSaveData data = (MoverSaveData)state;

            // set the position without the character's nav mesh agent interfering
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }

        /*
        // using dictionary to capture / restore data
        // member function required as we inherit from ISaveable
        public object CaptureState() {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        // member function required as we inherit from ISaveable
        public void RestoreState(object state) {
            // casting to SerializableVector3 because we know for sure that is what it is stored as
            Dictionary<string, object> data = (Dictionary<string, object>)state;

            // set the position without the character's nav mesh agent interfering
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
        */
    }
}
