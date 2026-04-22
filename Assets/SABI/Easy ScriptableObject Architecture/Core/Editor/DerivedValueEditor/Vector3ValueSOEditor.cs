using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.SOA
{

    [CustomEditor(typeof(BaseValueSO<Vector3>), true)]
    public class Vector3ValueSOEditor : Editor
    {
        public VisualTreeAsset tree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_ResetValue").clicked += ((BaseValueSO<Vector3>)target).ResetValue;
            customVisualElement.Q<Button>("DebugSetValue").clicked += ((BaseValueSO<Vector3>)target).DebugSetValue;
            customVisualElement.Q<Label>("AssetName").text = "Vector3 Value SO";

            return customVisualElement;
        }
    }
}