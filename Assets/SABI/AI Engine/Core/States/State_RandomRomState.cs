using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SABI
{
    public class State_RandomRom : State_Base
    {
        public float patrolRadius = 10f,
            patrolInterval = 15f,
            waitTimeBetweenNewPatroll = 5f,
            speed = 2;
        private Vector3 patrolPoint;
        NavmeshManager navmeshManager;
        bool isWaitingForNewPath;

        [SerializeField]
        private List<string> animationName_Idle;

        [SerializeField]
        private string animationName_Walk;

        private AnimationManager animationManager;

        public override void StateEnter()
        {
            base.StateEnter();
            animationManager =
                baseStateMachine.gameObject.GetComponentInChildren<AnimationManager>();
            StateMachine navmeshStateMachine = (StateMachine)baseStateMachine;
            navmeshManager = navmeshStateMachine.navMeshManager;
            navmeshManager.SetAgentSpeed(speed);
            SetNewPatrolPoint();
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            if (
                navmeshManager.agent.remainingDistance <= navmeshManager.agent.stoppingDistance
                && !navmeshManager.agent.pathPending
                && !isWaitingForNewPath
            )
            {
                animationManager.SetAnimation(animationNameList: animationName_Idle);
                SetNewPatrolPointWithDelay();
            }
        }

        public override void StateExit()
        {
            base.StateExit();
            navmeshManager.ResetPath();
        }

        private void SetNewPatrolPointWithDelay()
        {
            isWaitingForNewPath = true;
            Invoke(nameof(SetNewPatrolPoint), waitTimeBetweenNewPatroll);
        }

        private void SetNewPatrolPoint()
        {
            isWaitingForNewPath = false;
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position; // Center the random direction around the current position
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolPoint = hit.position;
                navmeshManager.SetDestination(patrolPoint);
                animationManager.SetAnimation(animationName_Walk);
            }
            else { }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, patrolRadius);
        }
    }
}
