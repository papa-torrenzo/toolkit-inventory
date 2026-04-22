using UnityEngine;
using UnityEngine.AI;

namespace SABI
{
    public class State_Patrol : State_Base
    {
        [SerializeField]
        private float waitTime = 2f;
        private float waitTimer = 0f;
        private bool waiting = false;
        private NavmeshManager navmeshManager;
        private AnimationManager animationManager; // 1. Add Animator reference

        [SerializeField]
        private SLinearPath patrolPoints;

        [SerializeField]
        private string walkAnimation = "walk",
            idleAnimation = "idle";
        private int currentMovePoint = 0;

        [SerializeField]
        private bool useCustomMinDistanceToTarget = false;

        [SerializeField, ShowIf(nameof(useCustomMinDistanceToTarget), true)]
        private float minDistanceToTarget = 0.5f;

        [SerializeField]
        private bool randomizeStartingPoint = true;

        [SerializeField]
        private float speed = 2,
            speedDeviation = 0.2f;
        bool oppositeDirection = false;

        public bool changePathAutomatically = true;

        [SerializeField, ShowIf(nameof(changePathAutomatically), true)]
        private Vector2 timeBeforeChangingPathRange = new Vector2(400f, 1200f);

        private float timeLeftBeforeChangingPath;

        public override void StateEnter()
        {
            base.StateEnter();
            if (changePathAutomatically)
                timeLeftBeforeChangingPath = Random.Range(
                    timeBeforeChangingPathRange.x,
                    timeBeforeChangingPathRange.y
                );
            if (patrolPoints == null)
                patrolPoints = SLinearPath.AllPatrolPoints.GetRandomItem();
            oppositeDirection = SUtilities.Chance(50);
            if (randomizeStartingPoint)
                currentMovePoint = Random.Range(0, patrolPoints.GetMaxPosition());
            StateMachine stateMachine = (StateMachine)baseStateMachine;
            navmeshManager = stateMachine.navMeshManager;
            animationManager = stateMachine.animationManager;

            if (navmeshManager == null)
            {
                Debug.LogError("NavMeshAgent component missing from NPC!");
                enabled = false;
                return;
            }

            SelectNextPoint();
            waiting = false;

            animationManager.SetAnimation(walkAnimation);
            navmeshManager.SetAgentSpeed(speed.RandomBias(speedDeviation));
        }

        public override void StateExit()
        {
            base.StateExit();
            waiting = false;

            navmeshManager.ResetPath();
            animationManager.SetAnimation(idleAnimation);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            if (waiting)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    waiting = false;
                    SelectNextPoint();
                    animationManager.SetAnimation(walkAnimation);
                }
                return;
            }

            if (
                !navmeshManager.agent.pathPending
                && navmeshManager.agent.remainingDistance
                    <= (
                        useCustomMinDistanceToTarget
                            ? minDistanceToTarget
                            : navmeshManager.agent.stoppingDistance
                    )
            )
            {
                if (!waiting)
                {
                    waiting = true;
                    waitTimer = waitTime;

                    animationManager.SetAnimation(idleAnimation);
                }
            }

            if (changePathAutomatically)
            {
                timeLeftBeforeChangingPath -= Time.deltaTime;
                if (timeLeftBeforeChangingPath <= 0)
                {
                    timeLeftBeforeChangingPath = Random.Range(
                        timeBeforeChangingPathRange.x,
                        timeBeforeChangingPathRange.y
                    );
                    patrolPoints = SLinearPath.AllPatrolPoints.GetRandomItem();
                    // Debug.Log($"[SAB] New Path Selecte");
                }
            }
        }

        // private void SelectNextPoint()
        // {
        //     navmeshManager.SetDestination(patrolPoints.GetNextPoint());
        //     navmeshManager.SetIsStoped(false);
        // }

        private void SelectNextPoint()
        {
            try
            {
                if (oppositeDirection)
                {
                    currentMovePoint--;
                    if (currentMovePoint < 0)
                        currentMovePoint = patrolPoints.GetMaxPosition() - 1;
                    if (currentMovePoint >= patrolPoints.GetMaxPosition())
                        currentMovePoint = 0;
                }
                else
                {
                    currentMovePoint++;
                    if (currentMovePoint >= patrolPoints.GetMaxPosition())
                        currentMovePoint = 0;
                    if (currentMovePoint >= patrolPoints.GetMaxPosition())
                        currentMovePoint = 0;
                }
                navmeshManager.SetDestination(patrolPoints.GetPosition(currentMovePoint));
                navmeshManager.SetIsStoped(false);
            }
            catch (System.Exception e)
            {
                Debug.Log(
                    $"[SAB] Found Error: {e} \n currentMovePoint: {currentMovePoint} | MaxPosition: {patrolPoints.GetMaxPosition()} oppositeDirection: {oppositeDirection}"
                );
            }
        }
    }
}
