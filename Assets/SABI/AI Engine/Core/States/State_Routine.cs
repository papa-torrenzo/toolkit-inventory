namespace SABI
{
    using System.Linq;
    using SABI;
    using UnityEngine;

    public class State_Routine : State_Base
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private StatusSystem status;
        private NavmeshManager navmeshManager;
        private AnimationManager animationManager; // 1. Add Animator reference

        [SerializeField]
        private State_Base normalState;

        [SerializeField]
        private UtilityProvidingObject currentInteractingObject = null;

        public enum RoutineType
        {
            None,
            NormalState,
            Need,
        }

        [SerializeField]
        private RoutineType routineType = RoutineType.Need;

        [
            SerializeField,
            Range(0, 1),
            Header(
                "----------------------------------------------------------------------------------"
            )
        ]
        private float blader,
            hunger,
            stamina,
            fun,
            curiosity;

        public override void StateEnter()
        {
            base.StateEnter();
            // status = GetComponentInChildren<StatusSystem>();
            StateMachine navmeshStateMachine = (StateMachine)baseStateMachine;
            navmeshManager = navmeshStateMachine.navMeshManager;
            animationManager =
                baseStateMachine.gameObject.GetComponentInChildren<AnimationManager>();
            normalState.baseStateMachine = baseStateMachine;
        }

        public override void StateUpdate()
        {
            base.StateUpdate();

            if (routineType == RoutineType.NormalState)
                normalState.StateUpdate();

            if (currentInteractingObject != null)
            {
                currentInteractingObject.Tick(OnComplete: () =>
                {
                    currentInteractingObject.InteractionCompleted(status);
                    currentInteractingObject = null;
                    Debug.Log($"[SAB] Interaction Completed");
                });

                return;
            }

            // if (status.bladder.Get() > 0.8f)
            // {
            //     // Toilet
            //     // if (routineType == RoutineType.NormalState)
            //     //     normalState.StateExit();
            //     Interact(StatusElementType.Bladder);
            // }
            // else
            if (status.hunger.Get() > 0.7f)
            {
                // Burger
                // if (routineType == RoutineType.NormalState)
                //     normalState.StateExit();
                Interact(StatusElementType.Hunger);
            }
            else if (status.stamina.Get() < 0.3f)
            {
                // Bead
                // if (routineType == RoutineType.NormalState)
                //     normalState.StateExit();
                Interact(StatusElementType.Stamina);
            }
            else if (status.fun.Get() < 0.3f)
            {
                // Arcade
                // if (routineType == RoutineType.NormalState)
                //     normalState.StateExit();
                Interact(StatusElementType.Fun);
            }
            else if (status.curiosity.Get() > 0.7f)
            {
                // VR

                Interact(StatusElementType.Curiosity);
            }
            else
            {
                if (routineType == RoutineType.Need)
                {
                    routineType = RoutineType.NormalState;
                    normalState.StateEnter();
                }
            }

            blader = status.bladder.Get();
            hunger = status.hunger.Get();
            stamina = status.stamina.Get();
            fun = status.fun.Get();
            curiosity = status.curiosity.Get();
        }

        public override void StateExit()
        {
            base.StateExit();
        }

        private UtilityProvidingObject FindClosestIntractableElement(
            StatusElementType statusElementType
        )
        {
            return UtilityProvidingObject
                .AllIntractableObjects.Where(item =>
                    item.isUsable
                    && item.intractableObjectDatas.Any(item2 =>
                        item2.statusElementType == statusElementType
                    )
                )
                .OrderBy(item => item.transform.Distance(transform.position))
                .FirstOrDefault();
        }

        public void Interact(StatusElementType statusElementType)
        {
            Debug.Log($"[SAB] Interact (StatusElementType: {statusElementType})");
            if (currentInteractingObject == null)
            {
                // navmeshManager.SetDestination(currentInteractingObject.GetMovePosition());
                UtilityProvidingObject interactingObject = FindClosestIntractableElement(
                    statusElementType
                );
                if (interactingObject != null)
                {
                    if (statusElementType == StatusElementType.Bladder)
                        Debug.Log($"[SAB] BLADDER 1");

                    if (routineType == RoutineType.NormalState)
                    {
                        if (statusElementType == StatusElementType.Bladder)
                            Debug.Log($"[SAB] BLADDER 2");

                        normalState.StateExit();
                        routineType = RoutineType.Need;
                    }

                    if (statusElementType == StatusElementType.Bladder)
                        Debug.Log($"[SAB] BLADDER 3");

                    if (transform.Distance(interactingObject.GetMovePosition()) > 0.3f)
                    {
                        if (statusElementType == StatusElementType.Bladder)
                            Debug.Log($"[SAB] BLADDER 4");

                        navmeshManager.SetDestination(interactingObject.GetMovePosition());
                        animationManager.SetAnimation("walk");
                    }
                    else
                    {
                        if (statusElementType == StatusElementType.Bladder)
                            Debug.Log($"[SAB] BLADDER 5");
                        animationManager.SetAnimation("idle");
                        navmeshManager.ResetPath();
                        animationManager.SetAnimation(interactingObject.animationName);
                        interactingObject.StartInteraction();
                        currentInteractingObject = interactingObject;
                        // interactingObject.interactionTimeLeft -= Time.deltaTime;
                        // if (interactingObject.interactionTimeLeft <= 0)
                        // {
                        //     interactingObject.InteractionCompleted(status);
                        // }
                    }
                    if (statusElementType == StatusElementType.Bladder)
                        Debug.Log($"[SAB] BLADDER 6");
                }
                else
                {
                    Debug.LogError("SABI: No Interactable Items");
                    if (routineType == RoutineType.Need)
                    {
                        routineType = RoutineType.NormalState;
                        normalState.StateEnter();
                    }
                    status.StatusDictionary[statusElementType].ResetToBetterExtremeValue();
                }
            }
        }
    }
}
