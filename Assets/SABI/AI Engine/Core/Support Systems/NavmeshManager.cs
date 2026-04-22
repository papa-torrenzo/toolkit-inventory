namespace SABI
{
    using System;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AI;

    [RequireComponent(typeof(NavMeshAgent))]
    public class NavmeshManager : MonoBehaviour
    {
        public NavMeshAgent agent { get; private set; }

        //#region Agent State
        public enum AgentState
        {
            Idle,
            ComputingPath,
            Moving,
            ReachedDestination,
            PathInvalid,
            PathPartial,
        }

        [SerializeField]
        private AgentState currentState;

        private AgentState DetermineState()
        {
            // 1. Is the path currently being computed?
            if (agent.pathPending)
                return AgentState.ComputingPath;

            // 2. Is the path valid?
            if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                return AgentState.PathInvalid;

            // 3. Is the path partial? (Target is unreachable, going as close as possible)
            if (agent.pathStatus == NavMeshPathStatus.PathPartial)
                return AgentState.PathPartial;

            // 4. Have we reached the destination?
            // Note: (!hasPath) is often true when we've reached the end
            if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
                return AgentState.ReachedDestination;

            // 5. If none of the above, we are moving
            return AgentState.Moving;
        }

        void Update()
        {
            currentState = DetermineState();
        }

        //#endregion
        private void Awake() => agent = GetComponent<NavMeshAgent>();

        void Start()
        {
            if (!agent.isOnNavMesh)
            {
                agent.transform.position = GetNearestNavMeshPoint(agent.transform.position);
            }

            // agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            // this.DelayedExecution(
            //     5,
            //     () =>
            //     {
            //         agent.obstacleAvoidanceType =
            //             ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            //     }
            // );
            agent.avoidancePriority = UnityEngine.Random.Range(0, 100);
        }

        public async void AddKnockBack(Vector3 sourcePosition, float knockback = -0.2f)
        {
            ResetPath();
            float agentSpeed = agent.speed;
            agent.speed = 10;
            Vector3 knockBackDirection =
                Vector3WithY(sourcePosition, 0) - Vector3WithY(transform.position, 0);
            SetDestination(transform.position + knockBackDirection.normalized * knockback);
            await Task.Delay(1000);
            agent.speed = agentSpeed;
        }

        public void SetAgentEnabled(bool value) => agent.enabled = value;

        public void ResetPath() => agent.ResetPath();

        public void SetStopingDistance(float value) => agent.stoppingDistance = value;

        bool canSetDestination = true;

        public void SetDestination(Vector3 target)
        {
            if (!canSetDestination)
                return;
            canSetDestination = false;
            this.DelayedExecution(0.3f, () => canSetDestination = true);

            if (agent.isOnNavMesh)
                agent.SetDestination(target);
            else
                agent.Warp(GetNearestNavMeshPoint(agent.transform.position));
        }

        public void SetClosestDestinationInNavmesh(Vector3 target)
        {
            SetDestination(GetNearestNavMeshPoint(target));
        }

        // public void SetDestination(Vector3 target)
        // {
        //     if (!agent.isOnNavMesh)
        //     {
        //         Debug.LogError("Movement: Agent is not on NavMesh.");
        //         agent.Warp(GetNearestNavMeshPoint(agent.transform.position));
        //         return;
        //     }

        //     // 1. Create a path object to store calculation results
        //     NavMeshPath path = new NavMeshPath();

        //     // 2. Calculate the path to the target
        //     // Note: CalculatePath is synchronous and can be expensive if called every frame
        //     if (agent.CalculatePath((target), path))
        //     {
        //         Debug.Log($"[SAB] [C] path.status: {path.status}");
        //         // 3. Check if the path is complete (reachable)
        //         if (path.status == NavMeshPathStatus.PathComplete)
        //         {
        //             agent.SetDestination(target);
        //         }
        //         else if (path.status == NavMeshPathStatus.PathPartial)
        //         {
        //             // 4. If partial, the last corner is the closest reachable point on the NavMesh
        //             Vector3 closestPoint = path.corners[path.corners.Length - 1];
        //             agent.SetDestination(closestPoint);

        //             Debug.LogWarning("Target unreachable. Moving to closest valid point instead.");
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogError("Movement: Failed to calculate even a partial path.", gameObject);
        //     }
        // }

        public void SetDestinationWithEvent(Vector3 target, float minDistance, Action OnReach)
        {
            SetDestination(target);
            Debug.Log(
                $"[SAB] NavManager {Vector3.Distance(transform.position, target)} < {minDistance}"
            );
            if (Vector3.Distance(transform.position, target) < minDistance)
            {
                ResetPath();
                OnReach?.Invoke();
            }
        }

        public void SetDestination(Transform target) => SetDestination(target.position);

        public bool IsNavMeshPathPossible(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(destination, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }

        public void SetAgentSpeed(float speed) => agent.speed = speed;

        public void SetIsStoped(bool value) => agent.isStopped = value;

        Vector3 Vector3WithY(Vector3 valueVector3, float valueY) =>
            new Vector3(valueVector3.x, valueY, valueVector3.z);

        Vector3 GetNearestNavMeshPoint(Vector3 position)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 100, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return position;
        }

        public void SetActivate(bool value)
        {
            return;
            agent.enabled = value;
            // ---------------------------------------------------------------------------------------------
            // agent.updateRotation = value;
            // if (value)
            // {
            // agent.angularSpeed = value ? 200 : 0;
            // }
        }
    }
}
