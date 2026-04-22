using UnityEditor;
using UnityEngine.UIElements;
namespace SABI.SOA
{
    [CustomEditor(typeof(EventSO), true)]
    public class EventSOEditor : Editor
    {
        public VisualTreeAsset tree;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement customVisualElement = new();
            tree.CloneTree(customVisualElement);
            customVisualElement.Q<Button>("Button_Invoke").clicked += ((EventSO)target).Invoke;
            return customVisualElement;
        }
    }
}


