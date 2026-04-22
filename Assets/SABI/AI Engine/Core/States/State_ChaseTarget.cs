using UnityEngine;

namespace SABI
{
    public class State_ChaseTarget : State_Base
    {
        NavmeshManager navmeshManager;
        Transform target;

        public void Init(Transform target)
        {
            Debug.Log($"[SAB] Init");
            this.target = target;
        }

        public override bool Validation(out string validationMessage)
        {
            Debug.Log($"[SAB] stateMachine is StateMachine: {baseStateMachine is StateMachine}");
            Debug.Log($"[SAB] target != null: {target != null}");
            validationMessage =
                $" stateMachine is StateMachine: {baseStateMachine is StateMachine}, target != null: {target != null} ";
            return baseStateMachine is StateMachine && target != null;
        }

        public override void StateEnter()
        {
            base.StateEnter();
            StateMachine navmeshStateMachine = (StateMachine)baseStateMachine;
            navmeshManager = navmeshStateMachine.navMeshManager;
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            if (target)
                navmeshManager.SetDestination(target);
            else
                Debug.LogError("No Target");
        }
    }
}
