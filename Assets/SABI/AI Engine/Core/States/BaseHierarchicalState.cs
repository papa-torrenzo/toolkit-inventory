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
    [CustomEditor(typeof(BaseHierarchicalState), true)]
    public class BaseHierarchicalStateEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            BaseHierarchicalState hierarchicalState = target as BaseHierarchicalState;
            var root = new VisualElement();
            VisualElement defaultElements = new();
            defaultElements
                .SetParent(root)
                .Border()
                .BorderWidth(1)
                .BGColor(Color.black.WithA(0.2f))
                .Padding(20)
                .MarginTopBottom(5);
            InspectorElement.FillDefaultInspector(defaultElements, serializedObject, this);

            Button addFolderStructureButton = new Button() { text = "Create Folder Structure" }
                .SetParent(root)
                .OnClick(() =>
                {
                    GameObject gameObject_States = new GameObject("States");
                    Undo.RegisterCreatedObjectUndo(gameObject_States, "Create States folder");
                    gameObject_States.transform.parent = hierarchicalState.transform;
                    gameObject_States.transform.SetLocalPositionAndRotation(
                        Vector3.zero,
                        Quaternion.identity
                    );

                    GameObject gameObject_Triggers = new GameObject("Transitions");
                    Undo.RegisterCreatedObjectUndo(
                        gameObject_Triggers,
                        "Create Transitions folder"
                    );
                    gameObject_Triggers.transform.parent = hierarchicalState.transform;
                    gameObject_Triggers.transform.SetLocalPositionAndRotation(
                        Vector3.zero,
                        Quaternion.identity
                    );

                    GameObject gameObject_Sensors = new GameObject("Sensors");
                    Undo.RegisterCreatedObjectUndo(gameObject_Sensors, "Create Sensors folder");
                    gameObject_Sensors.transform.parent = hierarchicalState.transform;
                    gameObject_Sensors.transform.SetLocalPositionAndRotation(
                        Vector3.zero,
                        Quaternion.identity
                    );

                    EditorSceneManager.MarkSceneDirty(hierarchicalState.gameObject.scene);
                    EditorUtility.SetDirty(hierarchicalState);
                });

            Button addNewStateButton = new Button() { text = "Add New State" }
                .SetParent(root)
                .OnClick(() =>
                {
                    GenericMenu menu = new();
                    foreach (var item in TypeCache.GetTypesDerivedFrom<State_Base>())
                    {
                        menu.AddItem(
                            new GUIContent(item.Name),
                            false,
                            () =>
                            {
                                Transform stateTransform = hierarchicalState.transform.Find(
                                    "States"
                                );
                                if (stateTransform)
                                {
                                    GameObject stateGameObject = new(item.Name);
                                    Undo.RegisterCreatedObjectUndo(stateGameObject, "Create State");
                                    stateGameObject.transform.SetParent(stateTransform);
                                    Undo.AddComponent(stateGameObject, item);
                                    stateGameObject.transform.localPosition = Vector3.zero;
                                }
                                else
                                {
                                    Undo.AddComponent(hierarchicalState.gameObject, item);
                                    EditorUtility.SetDirty(hierarchicalState);
                                }

                                EditorSceneManager.MarkSceneDirty(
                                    hierarchicalState.gameObject.scene
                                );
                            }
                        );
                    }
                    menu.ShowAsContext();
                });

            Button addNewTransitionButton = new Button() { text = "Add New Transition" }
                .SetParent(root)
                .OnClick(() =>
                {
                    GenericMenu menu = new();
                    foreach (var item in TypeCache.GetTypesDerivedFrom<Transition_Base>())
                    {
                        menu.AddItem(
                            new GUIContent(item.Name),
                            false,
                            () =>
                            {
                                Transform transitionTransform = hierarchicalState.transform.Find(
                                    "Transitions"
                                );
                                if (transitionTransform)
                                {
                                    GameObject transitionGameObject = new(item.Name);
                                    transitionGameObject.transform.SetParent(transitionTransform);
                                    transitionGameObject.gameObject.AddComponent(item);
                                }
                                else
                                    hierarchicalState.gameObject.AddComponent(item);
                            }
                        );
                    }
                    menu.ShowAsContext();
                });
            Button addNewSensorsButton = new Button() { text = "Add New Sensors" }
                .SetParent(root)
                .OnClick(() =>
                {
                    GenericMenu menu = new();
                    foreach (var item in TypeCache.GetTypesDerivedFrom<Sensor_Base>())
                    {
                        menu.AddItem(
                            new GUIContent(item.Name),
                            false,
                            () =>
                            {
                                Transform sensorTransform = hierarchicalState.transform.Find(
                                    "Sensors"
                                );
                                if (sensorTransform)
                                {
                                    GameObject sensorGameObject = new(item.Name);
                                    sensorGameObject.transform.SetParent(sensorTransform);
                                    sensorGameObject.gameObject.AddComponent(item);
                                }
                                else
                                    hierarchicalState.gameObject.AddComponent(item);
                            }
                        );
                    }
                    menu.ShowAsContext();
                });

            return root;
        }
    }
#endif
    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>

    public class BaseHierarchicalState : State_Base
    {
        public event Action<OldAndNewValue<State_Base>> OnStateChanged;

        public State_Base startingState;
        protected State_Base currentState;

        public override void StateEnter()
        {
            base.StateEnter();

            if (startingState)
                SetState(startingState);
            else
                SetState(GetComponentInChildren<State_Base>());
        }

        public override void StateUpdate()
        {
            base.StateUpdate();

            if (currentState)
                currentState.StateUpdate();
        }

        public override void StateExit()
        {
            base.StateExit();
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

            newState.baseStateMachine = baseStateMachine;

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
}
