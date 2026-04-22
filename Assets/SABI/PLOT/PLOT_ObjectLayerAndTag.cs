namespace SABI
{
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    public class PLOT_ObjectLayerAndTag : PLOT
    {
        public override void Execute() { }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT_ObjectLayerAndTag))]
    public class PLOT_ObjectLayerAndTagEditor : PLOTEditor { }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
