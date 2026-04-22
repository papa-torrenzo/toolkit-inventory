// using SABI.Flow;
// using UnityEngine;
// using UnityEngine.UIElements;
// #if UNITY_EDITOR

// using UnityEditor;

// public class ExtensionProWindow : EditorWindow
// {
//     [MenuItem("Window/ExtensionProWindow")]
//     private static void ShowWindow()
//     {
//         var window = GetWindow<ExtensionProWindow>();
//         window.titleContent = new GUIContent("ExtensionProWindow");
//         window.Show();
//     }

//     void CreateGUI()
//     {
//         VisualElement element = new();
//         Label lb = new();
//         lb.text = "I am GOD";
//         lb.FontSize(55).FontColor(Color.yellow);

//         SABI.Flow.Scrollable row = new(
//             new Rectangle(250).Expand().BGColorRandom(),
//             new Rectangle(250).Expand().BGColorRandom()
//         );

//         rootVisualElement.Add(
//             new Div()
//                 .Expand()
//                 .Border()
//                 .Expand()
//                 .Margin()
//                 .Insert(
//                     new Label("Extensions Pro")
//                         .FontColor(Color.yellow)
//                         .FontSize(50)
//                         .UnityTextAlignCenter(),
//                     // new Div().FixedSize(100).BGColorRandom().BorderRadius(),
//                     row.Expand().BorderRadius()
//                 )
//         );

//         // row.contentContainer.Add(new Rectangle(250).BGColorRandom());
//     }
// }

// #endif
