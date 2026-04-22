#if UNITY_EDITOR
using System.Linq;
using SABI.Flow;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;

namespace SABI
{
    public class WatcherWindow : EditorWindow
    {
        [MenuItem("Tools/Sabi/Watcher")]
        private static void ShowWindow()
        {
            var window = GetWindow<WatcherWindow>();
            window.titleContent = new GUIContent("Watcher");
            window.Show();
        }

        ScrollView scrollable;
        string searchKeyword;
        float keySectionWidth = 100;
        bool allowMultiLineMessage = false;

        VisualElement nothingIsWatchedElement;

        void CreateGUI()
        {
            rootVisualElement.Padding(20).PaddingBottom(0);

            rootVisualElement.Add(
                new FText("WATCHER", fontSize: 30, letterSpacing: 50)
                    .PaddingBottom(20)
                    .FixedHeight(50)
            );
            // rootVisualElement.Add( new Button( ()=> scrollable.contentContainer.Clear() ) { text = "Clear" } );

            TextField textField = new TextField() { label = "Search" };
            textField.RegisterValueChangedCallback(e => searchKeyword = e.newValue);
            rootVisualElement.Add(textField);

            Toggle allowMultilineToggle = new Toggle() { label = "Multi line" };
            allowMultilineToggle.RegisterValueChangedCallback(e =>
                allowMultiLineMessage = e.newValue
            );
            rootVisualElement.Add(allowMultilineToggle);

            Slider slider =
                new()
                {
                    lowValue = 0.25f,
                    highValue = 0.7f,
                    label = "Key text width",
                };
            slider.RegisterValueChangedCallback(e => keySectionWidth = e.newValue * position.width);
            rootVisualElement.Add(slider);

            scrollable = new();
            scrollable.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollable.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            rootVisualElement.Add(new Div(scrollable).MarginTop(20));

            nothingIsWatchedElement = new FText("Nothing is watched")
                .SetParent(rootVisualElement)
                .FixedHeight(50)
                .Border(10, 1, Color.white.WithA(.2f))
                .CenterF()
                .BGColor(Color.black.WithA(.2f));
        }

        void OnEnable() => EditorApplication.playModeStateChanged += HandlePlayModeStateChange;

        void OnDisable() => EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;

        private void HandlePlayModeStateChange(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredEditMode)
            {
                scrollable?.contentContainer?.Clear();
            }
        }

        void Update()
        {
            if (!Application.isEditor)
                return;
            if (!Application.isPlaying)
                return;

            // ---------------------------------------------------------------------------------------------

            scrollable.contentContainer.Clear();

            // ---------------------------------------------------------------------------------------------

            if (!Watcher.IsAnyWatcherAvailable())
            {
                scrollable.Hide();
                nothingIsWatchedElement.Show();
                return;
            }
            else
            {
                scrollable.Show();
                nothingIsWatchedElement.Hide();
            }

            // ---------------------------------------------------------------------------------------------

            HandleWatchType<string>(
                scrollable,
                Watcher.GetWatchList_String(),
                (value) =>
                    new FText(
                        value,
                        fontSize: 14,
                        textAnchor: TextAnchor.MiddleLeft,
                        wrap: allowMultiLineMessage
                    )
            );

            HandleWatchType<bool>(
                scrollable,
                Watcher.GetWatchList_Bool(),
                (thisValue) => new Toggle() { value = thisValue }
            );

            HandleWatchType<UnityEngine.Object>(
                scrollable,
                Watcher.GetWatchList_Object(),
                (thisValue) => new ObjectField() { value = thisValue }
            );

            // ---------------------------------------------------------------------------------------------

            // MockValues();
        }

        private void HandleWatchType<T>(
            VisualElement parent,
            Dictionary<string, Func<T>> data,
            Func<T, VisualElement> handleWatchType
        )
        {
            if (data == null || data.Count == 0)
                return;

            FColumn column = new();
            column
                .BGColor(Color.black.WithA(.2f))
                .Border(10, 1, Color.white.WithA(.2f))
                .Padding(0)
                .MarginBottom(15);

            foreach (var item in data)
            {
                if (
                    searchKeyword != null
                    && searchKeyword.Trim() != ""
                    && !item.Key.ToLower().Contains(searchKeyword.ToLower())
                )
                    continue;
                column.Add(
                    new FColumn(
                        new FRow(
                            new FText(
                                item.Key,
                                textAnchor: TextAnchor.MiddleLeft,
                                fontSize: 14,
                                wrap: allowMultiLineMessage
                            ).FixedWidth(keySectionWidth),
                            handleWatchType(item.Value())
                        )
                            .Padding(5)
                            .Margin(5, 10)
                            .MinHeight(40)
                            .AlignItems(Align.Center),
                        new FRectangle(Length.Percent(100), 1).BGColor(
                            data.Keys.Last() == item.Key ? Color.clear : Color.white.WithA(.2f)
                        )
                    )
                );
            }

            parent.Add(column);
        }

        #region Testing -------------------------------------------------------------------------- <Reg: Testing>

        private void MockValues()
        {
            HandleWatchType<string>(
                scrollable,
                new()
                {
                    { "1", () => "HI" },
                    { "2", () => "HI" },
                    { "3", () => "HI" },
                    {
                        "33",
                        () =>
                            "HI jdkjnsdfkjnkn kjnskjnjnj sjnkdfjns jnsjndnjnajnsj jnajndajndjn akjnksdjnka"
                    },
                    { "jnok oojioj ojn ojno ojn ojno ojn ojn o nojonn", () => "HI" },
                    { "Sabi", () => "HI" },
                    { "Sabi2", () => "HI" },
                    { "Sabi3", () => "HI" },
                    { "Sabi4", () => "HI" },
                    { "Sabi5", () => "HI" },
                    { "Sabi6", () => "HI" },
                    { "This is a test", () => "HI" },
                },
                (value) =>
                    new FText(
                        value,
                        fontSize: 14,
                        textAnchor: TextAnchor.MiddleLeft,
                        wrap: allowMultiLineMessage
                    )
            );

            HandleWatchType<bool>(
                scrollable,
                new()
                {
                    { "1", () => true },
                    { "2", () => true },
                    { "3", () => false },
                    { "Sabi", () => true },
                    { "This is a test", () => false },
                },
                (thisValue) => new Toggle() { value = thisValue }
            );

            HandleWatchType<UnityEngine.Object>(
                scrollable,
                new()
                {
                    { "1", () => this },
                    { "2", () => this },
                    { "3", () => this },
                    { "Sabi", () => this },
                    { "This is a test", () => this },
                },
                (thisValue) => new ObjectField() { value = thisValue }
            );
        }

        #endregion Testing ---------------------------------------------------------------------- </Reg: Testing>
    }
}
#endif
