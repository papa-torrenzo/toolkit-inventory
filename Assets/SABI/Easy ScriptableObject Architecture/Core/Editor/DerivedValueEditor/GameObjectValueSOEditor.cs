using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI.SOA
{

    [CustomEditor(typeof(BaseValueSO<GameObject>), true)]
    public class GameObjectValueSOEditor : Editor
    {
        public VisualTreeAsset tree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_ResetValue").clicked += ((BaseValueSO<GameObject>)target).ResetValue;
            customVisualElement.Q<Button>("DebugSetValue").clicked += ((BaseValueSO<GameObject>)target).DebugSetValue;
            customVisualElement.Q<Label>("AssetName").text = "GameObject Value SO";

            return customVisualElement;
        }
    }
}