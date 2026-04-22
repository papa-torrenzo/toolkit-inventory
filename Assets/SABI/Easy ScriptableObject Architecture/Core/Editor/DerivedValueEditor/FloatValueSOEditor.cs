using UnityEditor;
using UnityEngine.UIElements;

namespace SABI.SOA
{
    [CustomEditor(typeof(BaseValueSO<float>), true)]
    public class FloatValueSOEditor : Editor
    {
        public VisualTreeAsset tree;


        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_ResetValue").clicked += ((BaseValueSO<float>)target).ResetValue;
            customVisualElement.Q<Button>("DebugSetValue").clicked += ((BaseValueSO<float>)target).DebugSetValue;
            customVisualElement.Q<Label>("AssetName").text = "Float Value SO";
            return customVisualElement;
        }
    }
}
