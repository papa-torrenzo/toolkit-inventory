using System;
using System.Collections.Generic;
using SABI.Flow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using FColumn = SABI.Flow.FColumn;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/EventSO")]
    public class EventSO : ResettableSO
    {
        [SerializeField]
        protected string CustomName = "Event SO";

        [SerializeField]
        protected int totalSubscribers;

        [SerializeField]
        protected bool debugLog;

        [SerializeField]
        protected List<MonoBehaviour> MonoBehaviourSubscribers = new();

        [SerializeField]
        protected List<ScriptableObject> ScriptableObjectSubscribers = new();

        [SerializeField]
        protected List<Object> ObjectSubscribers = new();

        [SerializeField]
        protected string debugLogMessage = "EventSO Invoked";
        protected event Action OnAction;

        public void Invoke()
        {
            OnAction?.Invoke();
            if (debugLog)
                Debug.Log(debugLogMessage);
        }

        public void Subscribe(Object subscriber, Action callBack)
        {
            OnAction += callBack;
            totalSubscribers++;

            if (subscriber is MonoBehaviour mono)
                MonoBehaviourSubscribers.Add(mono);
            else if (subscriber is ScriptableObject scriptable)
                ScriptableObjectSubscribers.Add(scriptable);

            ObjectSubscribers.Add(subscriber);
        }

        public void UnSubscribe(Object subscriber, Action callBack)
        {
            OnAction -= callBack;
            totalSubscribers--;
            if (subscriber is MonoBehaviour mono)
                MonoBehaviourSubscribers.Remove(mono);
            else if (subscriber is ScriptableObject scriptable)
                ScriptableObjectSubscribers.Remove(scriptable);

            ObjectSubscribers.Remove(subscriber);
        }

        protected override void ResetSO()
        {
            base.ResetSO();
            MonoBehaviourSubscribers.Clear();
            ScriptableObjectSubscribers.Clear();
            ObjectSubscribers.Clear();
            totalSubscribers = 0;
        }
    }
}

#region Editor

#if UNITY_EDITOR

namespace SABI.SOA
{
    [CustomEditor(typeof(EventSO), true)]
    public class EventSOEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            string customName = serializedObject.FindProperty("CustomName").stringValue;
            Label customNameField = new(customName);

            customNameField
                .FontSize(33)
                .UnityTextAlignCenter()
                .Bold()
                .MarginLeftRight(2)
                .MarginTopBottom(22)
                .TextColor(Color.white);

            // ---------------------------------------------------------------------------------------------
            PropertyField totalSubscribers = new(serializedObject.FindProperty("totalSubscribers"));
            totalSubscribers.SetEnabled(false);
            Div subscribersSection =
                new(
                    new FColumn(
                        15,
                        0,
                        new Div().FixedHeight(10),
                        totalSubscribers,
                        new PropertyField(serializedObject.FindProperty("ObjectSubscribers")),
                        new PropertyField(
                            serializedObject.FindProperty("MonoBehaviourSubscribers")
                        ),
                        new PropertyField(
                            serializedObject.FindProperty("ScriptableObjectSubscribers")
                        )
                    )
                );

            subscribersSection.SetEnabled(false);
            subscribersSection.Opacity(1);
            // ---------------------------------------------------------------------------------------------
            Div actionsSection =
                new(
                    new FColumn(
                        15,
                        0,
                        new Div().FixedHeight(10),
                        new Button { text = "Invoke" }
                            .Expand()
                            .UnityTextAlignCenter()
                            .BorderRadius()
                            // .Height(50)
                            .BGColor(0.22f)
                            .Padding()
                            .OnHover(
                                element =>
                                    element
                                        .FontColor(Color.yellow)
                                        .LetterSpacing(25)
                                        .FontSize(22)
                                        .Height(60)
                                        .Bold(),
                                element =>
                                    element
                                        .FontColor(Color.white)
                                        .LetterSpacing(0)
                                        .FontSize(14)
                                        .Height(50)
                                        .NoFontStyle()
                            )
                            .OnClick(() =>
                            {
                                var method = target
                                    .GetType()
                                    .GetMethod(
                                        "Invoke",
                                        System.Reflection.BindingFlags.Instance
                                            | System.Reflection.BindingFlags.Public
                                            | System.Reflection.BindingFlags.NonPublic
                                    );
                                if (method != null)
                                    method.Invoke(target, null);
                                serializedObject.ApplyModifiedProperties();
                            }),
                        new Button { text = "Reset" }
                            .Expand()
                            .UnityTextAlignCenter()
                            .BorderRadius()
                            // .Height(50)
                            .BGColor(0.22f)
                            .Padding()
                            .OnHover(
                                element =>
                                    element
                                        .FontColor(Color.yellow)
                                        .LetterSpacing(25)
                                        .FontSize(22)
                                        .Height(60)
                                        .Bold(),
                                element =>
                                    element
                                        .FontColor(Color.white)
                                        .LetterSpacing(0)
                                        .FontSize(14)
                                        .Height(50)
                                        .NoFontStyle()
                            )
                            .OnClick(() =>
                            {
                                var method = target
                                    .GetType()
                                    .GetMethod(
                                        "Reset",
                                        System.Reflection.BindingFlags.Instance
                                            | System.Reflection.BindingFlags.Public
                                            | System.Reflection.BindingFlags.NonPublic
                                    );
                                if (method != null)
                                    method.Invoke(target, null);
                                serializedObject.ApplyModifiedProperties();
                            })
                    )
                );
            // ---------------------------------------------------------------------------------------------
            Div debuggingSection =
                new(
                    new FColumn(
                        15,
                        0,
                        new Div().FixedHeight(10),
                        new PropertyField(serializedObject.FindProperty("debugLog")),
                        new FText(
                            "Log Message [ Starting ] [ $Value ] [ Ending ]",
                            10,
                            Color.gray,
                            textAnchor: TextAnchor.MiddleLeft
                        ),
                        new PropertyField(serializedObject.FindProperty("debugLogMessage"))
                    )
                );

            Foldout subscribersFoldout = new() { text = "Subscribers" };
            subscribersFoldout.Add(subscribersSection);

            Foldout actionsFoldout = new() { text = "Actions" };
            actionsFoldout.Add(actionsSection);

            Foldout debuggingFoldout = new() { text = "Debugging" };
            debuggingFoldout.Add(debuggingSection);

            if (customName != null && customName.Trim() != "")
                root.Add(customNameField);
            else
                root.Add(new Div().FixedHeight(20));

            root.Add(SectionStyle(subscribersFoldout));
            root.Add(SectionStyle(actionsFoldout));
            root.Add(SectionStyle(debuggingFoldout));

            return root;
        }

        private VisualElement SectionStyle(VisualElement element)
        {
            return element
                .BGColor(new Color(0, 0, 0, 0.4f))
                .FixedWidth(Length.Auto())
                .Padding(25)
                .Margin(5)
                .BorderRadius(25);
        }
    }
}

#endif

#endregion
