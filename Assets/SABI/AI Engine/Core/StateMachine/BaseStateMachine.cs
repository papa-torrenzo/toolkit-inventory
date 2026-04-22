using System;
using System.Collections.Generic;
using SABI;
using SABI.Flow;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement; // added
#endif

namespace SABI
{
    #region Editor ------------------------------------------------------------------------- <Reg: Editor>
#if UNITY_EDITOR
    [CustomEditor(typeof(BaseStateMachine), true)]
    public class BaseStateMachineEditor : Editor
    {
        public VisualElement root;
        public VisualElement baseStateMachineControls;

        public override VisualElement CreateInspectorGUI()
        {
            BaseStateMachine stateMachine = target as BaseStateMachine;
            baseStateMachineControls = new();
            root = new();
            VisualElement defaultElements = new();
            defaultElements
                .SetParent(root)
                .Border()
                .BorderWidth(1)
                .BGColor(Color.black.WithA(0.2f))
                .Padding(20)
                .MarginTopBottom(5);
            InspectorElement.FillDefaultInspector(defaultElements, serializedObject, this);

            new FButtonSection(
                "State Machine",
                new Dictionary<string, Action>
                {
                    {
                        "Structure",
                        () =>
                        {
                            Transform statesTransform = stateMachine.transform.Find("States");
                            Transform triggerTransform = stateMachine.transform.Find("Transitions");
                            Transform sensorTransform = stateMachine.transform.Find("Sensors");
                            if (statesTransform == null)
                            {
                                GameObject gameObject_States = new GameObject("States");
                                Undo.RegisterCreatedObjectUndo(
                                    gameObject_States,
                                    "Create States folder"
                                );
                                gameObject_States.transform.parent = stateMachine.transform;
                                gameObject_States.transform.SetLocalPositionAndRotation(
                                    Vector3.zero,
                                    Quaternion.identity
                                );
                            }
                            if (triggerTransform == null)
                            {
                                GameObject gameObject_Triggers = new GameObject("Transitions");
                                Undo.RegisterCreatedObjectUndo(
                                    gameObject_Triggers,
                                    "Create Transitions folder"
                                );
                                gameObject_Triggers.transform.parent = stateMachine.transform;
                                gameObject_Triggers.transform.SetLocalPositionAndRotation(
                                    Vector3.zero,
                                    Quaternion.identity
                                );
                            }
                            if (sensorTransform == null)
                            {
                                GameObject gameObject_Sensors = new GameObject("Sensors");
                                Undo.RegisterCreatedObjectUndo(
                                    gameObject_Sensors,
                                    "Create Sensors folder"
                                );
                                gameObject_Sensors.transform.parent = stateMachine.transform;
                                gameObject_Sensors.transform.SetLocalPositionAndRotation(
                                    Vector3.zero,
                                    Quaternion.identity
                                );
                            }

                            EditorSceneManager.MarkSceneDirty(stateMachine.gameObject.scene);
                            EditorUtility.SetDirty(stateMachine);
                        }
                    },
                    {
                        "State",
                        () =>
                        {
                            GenericMenu menu = new();
                            foreach (var item in TypeCache.GetTypesDerivedFrom<State_Base>())
                            {
                                menu.AddItem(
                                    new GUIContent(item.Name),
                                    false,
                                    () =>
                                    {
                                        Transform stateTransform = stateMachine.transform.Find(
                                            "States"
                                        );

                                        if (stateTransform)
                                        {
                                            Undo.AddComponent(stateTransform.gameObject, item);
                                        }
                                        else
                                        {
                                            GameObject stateGameObject = new(item.Name);
                                            Undo.RegisterCreatedObjectUndo(
                                                stateGameObject,
                                                "Create State"
                                            );
                                            stateGameObject.transform.SetParent(stateTransform);
                                            Undo.AddComponent(stateGameObject, item);
                                            stateGameObject.transform.localPosition = Vector3.zero;

                                            Undo.AddComponent(stateMachine.gameObject, item);
                                        }

                                        EditorUtility.SetDirty(stateMachine);
                                        EditorSceneManager.MarkSceneDirty(
                                            stateMachine.gameObject.scene
                                        );
                                    }
                                );
                            }
                            menu.ShowAsContext();
                        }
                    },
                    {
                        "Transition",
                        () =>
                        {
                            GenericMenu menu = new();
                            foreach (var item in TypeCache.GetTypesDerivedFrom<Transition_Base>())
                            {
                                menu.AddItem(
                                    new GUIContent(item.Name),
                                    false,
                                    () =>
                                    {
                                        Transform transitionTransform = stateMachine.transform.Find(
                                            "Transitions"
                                        );
                                        if (transitionTransform)
                                        {
                                            GameObject transitionGameObject = new(item.Name);
                                            transitionGameObject.transform.SetParent(
                                                transitionTransform
                                            );
                                            transitionGameObject.gameObject.AddComponent(item);
                                        }
                                        else
                                            stateMachine.gameObject.AddComponent(item);
                                    }
                                );
                            }
                            menu.ShowAsContext();
                        }
                    },
                    {
                        "Sensors",
                        () =>
                        {
                            GenericMenu menu = new();
                            foreach (var item in TypeCache.GetTypesDerivedFrom<Sensor_Base>())
                            {
                                menu.AddItem(
                                    new GUIContent(item.Name),
                                    false,
                                    () =>
                                    {
                                        Transform sensorTransform = stateMachine.transform.Find(
                                            "Sensors"
                                        );
                                        if (sensorTransform)
                                        {
                                            GameObject sensorGameObject = new(item.Name);
                                            sensorGameObject.transform.SetParent(sensorTransform);
                                            sensorGameObject.gameObject.AddComponent(item);
                                        }
                                        else
                                            stateMachine.gameObject.AddComponent(item);
                                    }
                                );
                            }
                            menu.ShowAsContext();
                        }
                    },
                }
            ).SetParent(baseStateMachineControls);
            baseStateMachineControls.SetParent(root);
            return root;
        }
    }
#endif
    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
    public class BaseStateMachine : MonoBehaviour
    {
        public event Action<OldAndNewValue<State_Base>> OnStateChanged;

        public State_Base startingState;

        [SerializeField]
        protected State_Base currentState;

        [SerializeField]
        private List<Transition_Base> commonTransitions;

        public virtual void Start()
        {
            if (startingState)
                SetState(startingState);
            else
                SetState(GetComponentInChildren<State_Base>());
        }

        public virtual void Update()
        {
            if (currentState)
                currentState.StateUpdate();
        }

        public virtual void SetState(State_Base newState, bool canSetSameState = false)
        {
            if (newState == null)
            {
                Debug.LogError("SetState() new state cant be null");
                return;
            }

            if (newState == currentState && !canSetSameState)
                return;
            newState.baseStateMachine = this;
            if (!newState.Validation(out string validationMessage))
            {
                Debug.LogError(
                    $"SetState() new state validation is failed, validationMessage: {validationMessage}"
                );
                return;
            }
            else
            {
                Debug.Log($"[SAB] Validation Pass");
            }

            if (currentState)
                currentState.StateExit();
            State_Base previousState = currentState;
            currentState = newState;
            currentState.SetCommonTransitions(commonTransitions);
            currentState.StateEnter();

            OnStateChanged?.Invoke(
                new OldAndNewValue<State_Base> { oldValue = previousState, newValue = currentState }
            );
        }
    }

    public class OldAndNewValue<T>
    {
        public T oldValue;
        public T newValue;
    }
}
