using System.Collections.Generic;
using System.Reflection;
using SABI.Flow;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SABI
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class MonoBehaviourButtonInspector : ButtonInspector { }

    [CustomEditor(typeof(ScriptableObject), true)]
    [CanEditMultipleObjects]
    public class ScriptableObjectButtonInspector : ButtonInspector { }

    [CanEditMultipleObjects]
    public class ButtonInspector : Editor
    {
        private readonly Dictionary<string, List<VisualElement>> buttonGroups = new();
        private readonly Dictionary<string, object[]> methodParameters = new();

        public override VisualElement CreateInspectorGUI()
        {
            // Debug.Log($"[SAB] Now You See Me ?");
            // return new Rectangle().FixedSize(250, 250);

            VisualElement root = new();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            MethodInfo[] methods = target
                .GetType()
                .GetMethods(
                    BindingFlags.Instance
                        | BindingFlags.Static
                        | BindingFlags.Public
                        | BindingFlags.NonPublic
                );

            buttonGroups.Clear();

            foreach (MethodInfo method in methods)
            {
                ButtonAttribute attribute = method.GetCustomAttribute<ButtonAttribute>();

                if (attribute == null || attribute.buttonPlacement == ButtonPlacement.EditorWindow)
                    continue;

                ButtonDrawer.HandleButton(
                    attribute,
                    method,
                    root,
                    buttonGroups,
                    methodParameters,
                    target
                );
            }

            foreach (string item in buttonGroups.Keys)
                root.Add(new FRow(buttonGroups[item]));

            return root;
        }
    }
}
