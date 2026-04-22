namespace SABI
{
    using System;
    using System.Collections.Generic;
    using SABI.Flow;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    public class BehaviouralAI : StateMachine
    {
        public Transform target;

        [ReadOnly]
        float distanceToTarget;

        [SerializeField]
        private float attackRange = 10;

        [SerializeField]
        private float chaseRange = 50;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public enum State
        {
            Normal,
            Chase,
            Attack,
        };

        readonly State behavioralState = State.Normal;

        public State_Base normalState,
            chaseState,
            attackState;

        public override void Update()
        {
            distanceToTarget = transform.Distance(target);
            bool isInChaseRange = distanceToTarget < chaseRange;
            bool isInAttackRange = distanceToTarget < attackRange;

            switch (behavioralState)
            {
                case State.Normal:
                    if (isInChaseRange)
                        SetState(chaseState);
                    else if (isInAttackRange)
                        SetState(attackState);
                    break;
                case State.Chase:
                    if (isInAttackRange)
                        SetState(attackState);
                    else if (!isInChaseRange)
                        SetState(normalState);
                    break;
                case State.Attack:
                    if (!isInAttackRange)
                    {
                        if (isInChaseRange)
                            SetState(chaseState);
                        else
                            SetState(normalState);
                    }
                    break;
            }

            base.Update();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BehaviouralAI))]
    public class BehaviouralAIEditor : Editor
    {
        BehaviouralAI behaviouralAI;

        public override VisualElement CreateInspectorGUI()
        {
            behaviouralAI = target as BehaviouralAI;

            VisualElement root = new VisualElement();

            // 1. Get the iterator for the serialized object
            SerializedProperty property = serializedObject.GetIterator();

            // 2. Move to the first property (usually m_Script)
            if (property.NextVisible(true))
            {
                do
                {
                    if (
                        property.name == "commonTransitions"
                        || property.name == "startingState"
                        || property.name == "currentState"
                        || property.name == "m_Script"
                    )
                    {
                        continue;
                    }

                    PropertyField field = new PropertyField(property);
                    root.Add(field);
                } while (property.NextVisible(false));
            }

            new FButtonSection(
                "Actions",
                new Dictionary<string, Action>()
                {
                    { "Add Normal State", () => AddAndSetState(BehaviouralAI.State.Normal) },
                    { "Add ChaseState", () => AddAndSetState(BehaviouralAI.State.Chase) },
                    { "Add AttackState", () => AddAndSetState(BehaviouralAI.State.Attack) },
                }
            ).SetParent(root).MarginTop(15);

            return root;
        }

        private void AddAndSetState(BehaviouralAI.State state)
        {
            GenericMenu menu = new();
            foreach (var item in TypeCache.GetTypesDerivedFrom<State_Base>())
            {
                menu.AddItem(
                    new GUIContent(item.Name),
                    false,
                    () =>
                    {
                        State_Base newState =
                            Undo.AddComponent(behaviouralAI.gameObject, item) as State_Base;
                        switch (state)
                        {
                            case BehaviouralAI.State.Normal:
                                behaviouralAI.normalState = newState;
                                break;
                            case BehaviouralAI.State.Chase:
                                behaviouralAI.chaseState = newState;
                                break;
                            case BehaviouralAI.State.Attack:
                                behaviouralAI.attackState = newState;
                                break;
                        }
                    }
                );
            }
            menu.ShowAsContext();
        }
    }
#endif
}
