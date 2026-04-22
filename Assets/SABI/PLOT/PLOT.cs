namespace SABI
{
    // PLOT : Procedural Layout & Object Tool

    using System;
    using System.Collections.Generic;
    using SABI.Flow;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    public abstract class PLOT : MonoBehaviour
    {
        public static List<PLOT> SLInstances = new();
        public abstract void Execute();
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT))]
    public class PLOTEditor : Editor
    {
        public Dictionary<string, Action> buttons;

        public VisualElement root;

        public override VisualElement CreateInspectorGUI()
        {
            buttons = new Dictionary<string, Action>();
            root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            buttons.Add("Execute", () => (target as PLOT).Execute());

            CustomThings();

            new FButtonSection("Actions", buttons).SetParent(root);

            return root;
        }

        public virtual void CustomThings() { }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
