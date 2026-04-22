using UnityEditor;
using UnityEngine.UIElements;

namespace SABI.SOA
{
    [CustomEditor(typeof(BaseValueSO<bool>), true)]
    public class BoolValueSOEditor : Editor
    {
        public VisualTreeAsset tree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_ResetValue").clicked += ((BaseValueSO<bool>)target).ResetValue;
            customVisualElement.Q<Button>("DebugSetValue").clicked += ((BaseValueSO<bool>)target).DebugSetValue;
            customVisualElement.Q<Label>("AssetName").text = "Bool Value SO";

            return customVisualElement;
        }
    }
}