namespace SABI
{
    using SABI;
    using UnityEngine;

    public class State_Flee : State_Base
    {
        [SerializeField]
        private Transform targetToFleeFrom;

        [SerializeField]
        private float fleeSpeed = 5;

        [SerializeField]
        private float directionMultiplayer = 5;

        Vector3 directionToFlee,
            positionToFlee;

        NavmeshManager navmeshManager;
        private AnimationManager animationManager;

        [SerializeField]
        private string animationName;

        public override void StateEnter()
        {
            base.StateEnter();
            animationManager =
                baseStateMachine.gameObject.GetComponentInChildren<AnimationManager>();
            StateMachine navmeshStateMachine = (StateMachine)baseStateMachine;
            navmeshManager = navmeshStateMachine.navMeshManager;
            navmeshManager.SetAgentSpeed(fleeSpeed);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            directionToFlee = (
                baseStateMachine.transform.position - targetToFleeFrom.position
            ).normalized;
            positionToFlee = transform.position + (directionMultiplayer * directionToFlee);
            navmeshManager.SetDestination(positionToFlee);
            animationManager.SetAnimation(animationName: animationName);
        }

        void OnDrawGizmos()
        {
            if (IsActive)
                Gizmos.DrawSphere(positionToFlee, 0.5f);
        }
    }
}
