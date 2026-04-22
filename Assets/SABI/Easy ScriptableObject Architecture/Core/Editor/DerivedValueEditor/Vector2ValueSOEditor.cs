using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.SOA
{

    [CustomEditor(typeof(BaseValueSO<Vector2>), true)]
    public class Vector2ValueSOEditor : Editor
    {
        public VisualTreeAsset tree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_ResetValue").clicked += ((BaseValueSO<Vector2>)target).ResetValue;
            customVisualElement.Q<Button>("DebugSetValue").clicked += ((BaseValueSO<Vector2>)target).DebugSetValue;
            customVisualElement.Q<Label>("AssetName").text = "Vector2 Value SO";

            return customVisualElement;
        }
    }
}