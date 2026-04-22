using UnityEditor;
using UnityEngine.UIElements;

namespace SABI.SOA
{
    [CustomEditor(typeof(BaseValueSO<int>), true)]
    public class IntValueSOEditor : Editor
    {
        public VisualTreeAsset tree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_ResetValue").clicked += ((BaseValueSO<int>)target).ResetValue;
            customVisualElement.Q<Button>("DebugSetValue").clicked += ((BaseValueSO<int>)target).DebugSetValue;
            customVisualElement.Q<Label>("AssetName").text = "Int Value SO";

            return customVisualElement;
        }
    }
}