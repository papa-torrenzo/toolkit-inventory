namespace SABI
{
    using System;
    using SABI;
    using UnityEngine;

    public class State_MoveAndInteract : State_Base
    {
        public int debugIndex = 0;
        StateMachine stateMachine;

        [SerializeField]
        private float distanceToInteractable = 0;

        public enum SubState
        {
            None,
            WalkingToInteractable,
            Interacting,
        }

        Action OnComplete;

        [SerializeField]
        private SubState subState = SubState.None;

        [SerializeField]
        private UtilityProvidingObject interactingObject;

        public override void StateEnter()
        {
            base.StateEnter();

            if (interactingObject == null)
                Debug.LogError("interactingObject = null");

            stateMachine = baseStateMachine as StateMachine;
            subState = SubState.None;
            // stateMachine.navMeshManager.SetDestination(interactingObject.GetMovePosition());
            // stateMachine.animationManager.SetAnimation("walk");
        }

        [SerializeField]
        private float lerpSpeed = 25;

        public override void StateUpdate()
        {
            base.StateUpdate();

            if (interactingObject == null)
                return;

            distanceToInteractable = transform.Distance(interactingObject.GetMovePosition());
            switch (subState)
            {
                case SubState.None:
                    stateMachine.navMeshManager.SetActivate(true);
                    subState = SubState.WalkingToInteractable;
                    stateMachine.navMeshManager.SetClosestDestinationInNavmesh(
                        interactingObject.GetMovePosition()
                    );
                    Debug.Log($"[SAB] SetPositionToBurger");
                    stateMachine.animationManager.SetAnimation("walk");
                    break;
                case SubState.WalkingToInteractable:
                    if (transform.Distance(interactingObject.GetMovePosition()) < 2f)
                    {
                        stateMachine.navMeshManager.ResetPath();
                        stateMachine.navMeshManager.SetActivate(false);
                        subState = SubState.Interacting;
                        stateMachine.animationManager.SetAnimation(interactingObject.animationName);

                        interactingObject.StartInteraction();
                    }
                    else
                    {
                        stateMachine.navMeshManager.SetDestination(
                            interactingObject.transform.position
                        );
                    }
                    break;
                case SubState.Interacting:

                    stateMachine.navMeshManager.SetActivate(false);

                    stateMachine.transform.position = Vector3.MoveTowards(
                        transform.position,
                        interactingObject.GetMovePosition(),
                        Time.deltaTime * lerpSpeed
                    );
                    Debug.Log(
                        $"[SAB] [A] interactingObject.GetRotation(): {interactingObject.GetRotation()}"
                    );
                    stateMachine.transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        Quaternion.Euler(interactingObject.GetRotation()),
                        Time.deltaTime * lerpSpeed
                    );

                    interactingObject.Tick(() =>
                    {
                        OnComplete();
                        subState = SubState.None;
                        stateMachine.navMeshManager.SetActivate(true);
                        distanceToInteractable = 99999;
                    });
                    break;
            }
        }

        public override void StateExit()
        {
            stateMachine.navMeshManager.SetActivate(true);
            base.StateExit();
        }

        public void SetInteractableObject(
            UtilityProvidingObject interactingObject,
            Action OnComplete
        )
        {
            debugIndex++;
            subState = SubState.None;
            this.interactingObject = interactingObject;
            this.OnComplete = OnComplete;
        }
    }
}
