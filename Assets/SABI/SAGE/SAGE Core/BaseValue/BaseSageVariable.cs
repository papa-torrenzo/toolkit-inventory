using System;
using System.Collections.Generic;
using SABI.Flow;
using UnityEngine;
using UnityEngine.UIElements;
using FColumn = SABI.Flow.FColumn;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
#endif

//TODO: Force set value

namespace SABI.SOA
{
    public abstract class BaseSageVariable<T> : ResettableSO
    {
        [SerializeField]
        protected string CustomName;

        [SerializeField]
        private T currentValue;

        [SerializeField]
        protected T newSetValueButtonArgument;

        [SerializeField]
        private bool eventOnReset;

        [SerializeField]
        protected T defaultValue;

        [SerializeField]
        private int totalSubscribers;

        [SerializeField]
        private List<MonoBehaviour> MonoBehaviourSubscribers = new();

        [SerializeField]
        private List<ScriptableObject> ScriptableObjectSubscribers = new();

        [SerializeField]
        private List<Object> ObjectSubscribers = new();

        // [SerializeField]
        // private T debugValue;

        [SerializeField]
        private bool debugLog;

        [SerializeField]
        private string debugLogMessageStarting;

        [SerializeField]
        private string debugLogMessageEnding;

        private event Action<T, T> OnValueChange;

        public void SetNewSetValueButtonArgumentAsNewValue() => SetValue(newSetValueButtonArgument);

        public void SetValue(T newValue)
        {
            OnValueChange?.Invoke(currentValue, newValue);
            currentValue = newValue;
            if (debugLog)
                Debug.Log($" {debugLogMessageStarting} {currentValue} {debugLogMessageEnding} ");
        }

        public T GetValue()
        {
            return currentValue;
        }

        public bool CompareValue(T value)
        {
            return EqualityComparer<T>.Default.Equals(currentValue, value);
        }

        public void SetResetValue(T newResetValue)
        {
            defaultValue = newResetValue;
        }

        protected override void ResetSO()
        {
            Debug.Log($"[SAB] Reset ResetSO");
            if (eventOnReset)
                SetValue(defaultValue);
            else
                currentValue = defaultValue;
        }

        // public void DebugSetValue() { SetValue(debugValue); }
        /// <summary>
        /// Subsribe and get notified to Value Change Events
        /// </summary>
        /// <param name="subscriber"> The subscriber instance reference </param>
        /// <param name="callBack"> Function that will take the old value and new value as parameters </param>
        public void Subscribe(Object subscriber, Action<T, T> callBack)
        {
            OnValueChange += callBack;
            totalSubscribers++;
            if (subscriber is MonoBehaviour)
                MonoBehaviourSubscribers.Add((MonoBehaviour)subscriber);
            else if (subscriber is ScriptableObject)
                ScriptableObjectSubscribers.Add((ScriptableObject)subscriber);
            ObjectSubscribers.Add(subscriber);
        }

        public void UnSubscribe(Object subscriber, Action<T, T> callBack)
        {
            OnValueChange -= callBack;
            totalSubscribers--;
            if (subscriber is MonoBehaviour)
                MonoBehaviourSubscribers.Remove((MonoBehaviour)subscriber);
            else if (subscriber is ScriptableObject)
                ScriptableObjectSubscribers.Remove((ScriptableObject)subscriber);
            ObjectSubscribers.Remove(subscriber);
        }

        //Todo: Non Object Subscription

        // Action<T> callback
    }
}

#region Editor

#if UNITY_EDITOR

namespace SABI.SOA
{
    // [CustomEditor(typeof(BaseValueSO<bool>), true)]
    public class BaseSageVariableEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            string customName = CustomNameSection(out Label customNameField);
            // ---------------------------------------------------------------------------------------------
            Div valueSection = ValueSection();
            // ---------------------------------------------------------------------------------------------
            Foldout subscribersFoldout = new() { text = "Subscribers" };
            subscribersFoldout.Add(SubscribersSection());
            // ---------------------------------------------------------------------------------------------
            Foldout actionsFoldout = new() { text = "Actions" };
            actionsFoldout.Add(ActionsSection());
            // ---------------------------------------------------------------------------------------------
            Foldout debuggingFoldout = new() { text = "Debugging" };
            debuggingFoldout.Add(DebuggingSection());
            // ---------------------------------------------------------------------------------------------

            VisualElement root = new();

            if (customName != null && customName.Trim() != "")
                root.Add(customNameField);
            else
                root.Add(new Div().FixedHeight(20));

            root.Insert(
                SectionStyle(valueSection),
                SectionStyle(subscribersFoldout),
                SectionStyle(actionsFoldout),
                SectionStyle(debuggingFoldout)
            );

            root.Bind(new SerializedObject(target));
            return root;
        }

        private string CustomNameSection(out Label customNameField)
        {
            string customName = serializedObject.FindProperty("CustomName").stringValue;
            customNameField = new Label(customName);

            customNameField
                .FontSize(33)
                .UnityTextAlignCenter()
                .Bold()
                .MarginLeftRight(2)
                .MarginTopBottom(22)
                .TextColor(Color.white);
            return customName;
        }

        private Div ValueSection()
        {
            PropertyField valueSection =
                new(serializedObject.FindProperty("currentValue"), "Value");

            PropertyField defaultValueSection = new PropertyField(
                serializedObject.FindProperty("defaultValue"),
                "Default Value"
            );

            defaultValueSection.tooltip = "This value can be used to reset the SO";

            valueSection.Disable();
            valueSection.Opacity(1);

            return new FColumn(15, 0, valueSection, defaultValueSection);
        }

        private Div DebuggingSection()
        {
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
                        new PropertyField(serializedObject.FindProperty("debugLogMessageStarting")),
                        new PropertyField(serializedObject.FindProperty("debugLogMessageEnding")),
                        new Div().FixedHeight(10),
                        new PropertyField(
                            serializedObject.FindProperty("newSetValueButtonArgument"),
                            "Set Value"
                        ),
                        Button(
                            "Set Value",
                            () =>
                            {
                                var method = target
                                    .GetType()
                                    .GetMethod(
                                        "SetNewSetValueButtonArgumentAsNewValue",
                                        System.Reflection.BindingFlags.Instance
                                            | System.Reflection.BindingFlags.Public
                                    );
                                if (method != null)
                                    method.Invoke(target, null);
                                serializedObject.ApplyModifiedProperties();
                            }
                        )
                    )
                );
            return debuggingSection;
        }

        private Div ActionsSection()
        {
            Div actionsSection =
                new(
                    new FColumn(
                        15,
                        0,
                        new Div().FixedHeight(10),
                        new PropertyField(serializedObject.FindProperty("eventOnReset")),
                        Button(
                            "Reset Value To Default",
                            () =>
                            {
                                var method = target
                                    .GetType()
                                    .GetMethod(
                                        "ResetSO",
                                        System.Reflection.BindingFlags.Instance
                                            | System.Reflection.BindingFlags.Public
                                    );
                                if (method != null)
                                    method.Invoke(target, null);
                                serializedObject.ApplyModifiedProperties();
                            }
                        )
                    )
                );
            return actionsSection;
        }

        private Div SubscribersSection()
        {
            PropertyField totalSubscribers = new(serializedObject.FindProperty("totalSubscribers"));
            totalSubscribers.SetEnabled(false);
            Div subscribersSection =
                new(
                    new FColumn(
                        15,
                        0,
                        new Div().FixedHeight(10),
                        totalSubscribers,
                        new PropertyField(serializedObject.FindProperty("ObjectSubscribers"))
                    // new PropertyField(serializedObject.FindProperty("MonoBehaviourSubscribers")),
                    // new PropertyField(serializedObject.FindProperty("ScriptableObjectSubscribers"))
                    )
                );

            subscribersSection.SetEnabled(false);
            subscribersSection.Opacity(1);
            return subscribersSection;
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

        private Label Button(string name, Action OnClick)
        {
            return new Label(name)
                .Expand()
                .UnityTextAlignCenter()
                .BorderRadius()
                .BGColor(0.22f)
                .Padding()
                .OnHover(
                    element =>
                        element
                            .FontColor(Color.yellow)
                            .LetterSpacing(5)
                            .FontSize(20)
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
                .OnClick(OnClick);
        }
    }
}
#endif

#endregion
