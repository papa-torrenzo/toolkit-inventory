using UnityEditor;
using UnityEngine.UIElements;

namespace SABI.SOA
{
    [CustomEditor(typeof(BaseValueSO<string>), true)]
    public class StringValueSOEditor : Editor
    {
        public VisualTreeAsset tree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_ResetValue").clicked += ((BaseValueSO<string>)target).ResetValue;
            customVisualElement.Q<Button>("DebugSetValue").clicked += ((BaseValueSO<string>)target).DebugSetValue;
            customVisualElement.Q<Label>("AssetName").text = "String Value SO";

            return customVisualElement;
        }
    }
}