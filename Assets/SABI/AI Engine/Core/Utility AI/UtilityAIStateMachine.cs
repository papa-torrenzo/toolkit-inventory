namespace SABI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SABI;
    using SABI.Flow;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;

#endif

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>
#if UNITY_EDITOR
    [CustomEditor(typeof(UtilityAIStateMachine), true)]
    public class UtilityAIStateMachineEditor : BaseStateMachineEditor
    {
        public VisualElement stateMachineControls;

        public override VisualElement CreateInspectorGUI()
        {
            base.CreateInspectorGUI();

            UtilityAIStateMachine stateMachine = target as UtilityAIStateMachine;
            stateMachineControls = new VisualElement();
            baseStateMachineControls.SetParent(stateMachineControls);

            new FButtonSection(
                "Status System",
                new Dictionary<string, Action>
                {
                    {
                        "Add Status System",
                        () =>
                        {
                            if (
                                stateMachine.TryGetComponentInChildren(
                                    out StatusSystem statusSystem
                                )
                            )
                            {
                                stateMachine.SetStatusSystem(statusSystem);
                                return;
                            }

                            GameObject gameObject_statusSystem = new GameObject("Status System");
                            Undo.RegisterCreatedObjectUndo(
                                gameObject_statusSystem,
                                "Add Status System"
                            );
                            gameObject_statusSystem.transform.parent = stateMachine.transform;
                            gameObject_statusSystem.transform.SetLocalPositionAndRotation(
                                Vector3.zero,
                                Quaternion.identity
                            );

                            Component addedComponents = Undo.AddComponent(
                                gameObject_statusSystem,
                                typeof(StatusSystem)
                            );

                            stateMachine.SetStatusSystem(addedComponents as StatusSystem);

                            EditorSceneManager.MarkSceneDirty(stateMachine.gameObject.scene);
                            EditorUtility.SetDirty(stateMachine);
                        }
                    },
                    {
                        "Add Status Debugger",
                        () =>
                        {
                            Undo.AddComponent(
                                stateMachine.GetComponentInChildren<StatusSystem>().gameObject,
                                typeof(DebugStatusSystem)
                            );

                            EditorSceneManager.MarkSceneDirty(stateMachine.gameObject.scene);
                            EditorUtility.SetDirty(stateMachine);
                        }
                    },
                    {
                        "Enable Status Debugging",
                        () =>
                        {
                            stateMachine.GetComponentInChildren<StatusSystem>().EnableBehaviour();
                        }
                    },
                    {
                        "Disable Status Debugging",
                        () =>
                        {
                            stateMachine.GetComponentInChildren<StatusSystem>().DisableBehaviour();
                        }
                    },
                }
            ).SetParent(stateMachineControls);

            stateMachineControls.SetParent(root);

            return root;
        }
    }
#endif
    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>


    public class UtilityAIStateMachine : StateMachine
    {
        [SerializeField]
        private StatusSystem status;

        [SerializeField]
        private State_Base state_patrol;

        [SerializeField]
        private State_MoveAndInteract state_moveAndInteract;

        [SerializeField]
        private UtilityProvidingObject currentInteractingObject = null;

        void Awake()
        {
            if (!status)
                status = GetComponentInChildren<StatusSystem>();

            state_patrol.baseStateMachine = this;
        }

        float delayForUpdate = 0.2f;
        float timeTillNextUpdate = 0;

        public override void Update()
        {
            base.Update();
            // timeTillNextUpdate -= Time.deltaTime;
            // if (timeTillNextUpdate <= 0)
            // {
            // timeTillNextUpdate = delayForUpdate;
            TimerBasedUpdate();
            // }
        }

        [SerializeField]
        private bool isAlredyInteracting = false;

        private void TimerBasedUpdate()
        {
            // SetState(state_patrol);

            // return;

            FindTheBestHeightsPriorityAction();

            if (highestUtilityValue > 0.5f)
            {
                if (currentInteractingObject != null)
                {
                    currentInteractingObject.intractableObjectDatas.ForEach(item =>
                    {
                        if (item.statusElementType == highestUtilityStatusData.StatusType)
                        {
                            isAlredyInteracting = true;
                            return;
                        }
                    });
                }

                if (isAlredyInteracting)
                    return;

                UtilityProvidingObject intractableObject = FindClosestIntractableElement(
                    highestUtilityStatusData.StatusType
                );

                // ---------------------------------------------------------------------------------- TODO: Change Later
                if (intractableObject != null)
                    intractableObject.isUsable = false;

                if (intractableObject == null)
                {
                    // status.OnInteract(highestUtilityStatusData);
                }

                // ---------------------------------------------------------------------------------------------

                if (intractableObject != null)
                {
                    currentInteractingObject = intractableObject;

                    state_moveAndInteract.SetInteractableObject(
                        intractableObject,
                        OnComplete: () =>
                        {
                            intractableObject.InteractionCompleted(status);
                            highestUtilityValue = 0;
                            highestUtilityStatusData = null;
                            currentInteractingObject = null;
                            isAlredyInteracting = false;
                            SetState(state_patrol);
                        }
                    );
                    SetState(state_moveAndInteract);
                }
                else
                {
                    SetState(state_patrol);
                }
            }
            else
            {
                if (currentState != state_patrol)
                    SetState(state_patrol);
            }
        }

        public override void SetState(State_Base newState, bool canSetSameState = false)
        {
            base.SetState(newState, canSetSameState);
        }

        [SerializeField]
        private StatusData highestUtilityStatusData;

        [SerializeField]
        private float highestUtilityValue = 0;

        private void FindTheBestHeightsPriorityAction()
        {
            status
                .StatusDictionary.Values.ToList()
                .ForEach(item =>
                {
                    float utilityValue = item.GetUtilityValue();
                    if (utilityValue > highestUtilityValue)
                    {
                        highestUtilityStatusData = item;
                        highestUtilityValue = utilityValue;
                    }
                });
        }

        public void SetStatusSystem(StatusSystem statusSystem) => this.status = statusSystem;

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

        // public void Interact(StatusElementType statusElementType)
        // {
        //     Debug.Log($"[SAB] Interact (StatusElementType: {statusElementType})");
        //     if (currentInteractingObject == null)
        //     {
        //         // navmeshManager.SetDestination(currentInteractingObject.GetMovePosition());
        //         IntractableObject interactingObject = FindClosestIntractableElement(
        //             statusElementType
        //         );
        //         if (interactingObject != null)
        //         {
        //             if (statusElementType == StatusElementType.Bladder)
        //                 Debug.Log($"[SAB] BLADDER 1");

        //             if (statusElementType == StatusElementType.Bladder)
        //                 Debug.Log($"[SAB] BLADDER 3");

        //             if (transform.Distance(interactingObject.GetMovePosition()) > 0.3f)
        //             {
        //                 if (statusElementType == StatusElementType.Bladder)
        //                     Debug.Log($"[SAB] BLADDER 4");

        //                 navMeshManager.SetDestination(interactingObject.GetMovePosition());
        //                 animationManager.SetAnimation("walk");
        //             }
        //             else
        //             {
        //                 if (statusElementType == StatusElementType.Bladder)
        //                     Debug.Log($"[SAB] BLADDER 5");
        //                 animationManager.SetAnimation("idle");
        //                 navMeshManager.ResetPath();
        //                 animationManager.SetAnimation(interactingObject.animationName);
        //                 interactingObject.StartInteraction();
        //                 currentInteractingObject = interactingObject;
        //             }
        //             if (statusElementType == StatusElementType.Bladder)
        //                 Debug.Log($"[SAB] BLADDER 6");
        //         }
        //     }
        // }
    }
}
