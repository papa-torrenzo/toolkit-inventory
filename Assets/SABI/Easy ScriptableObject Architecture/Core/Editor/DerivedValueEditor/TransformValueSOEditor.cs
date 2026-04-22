using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.SOA
{
    [CustomEditor(typeof(BaseValueSO<Transform>), true)]
    public class TransformValueSOEditor : Editor
    {
        public VisualTreeAsset tree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_ResetValue").clicked += ((BaseValueSO<Transform>)target).ResetValue;
            customVisualElement.Q<Button>("DebugSetValue").clicked += ((BaseValueSO<Transform>)target).DebugSetValue;
            customVisualElement.Q<Label>("AssetName").text = "Transform Value SO";

            return customVisualElement;
        }
    }
}