namespace SABI
{
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    public class PLOT_PhysicsSettle : PLOT
    {
        public override void Execute() { }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT_PhysicsSettle))]
    public class PLOT_PhysicsSettleEditor : PLOTEditor { }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
