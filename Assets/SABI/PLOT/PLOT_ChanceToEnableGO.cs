namespace SABI
{
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using Unity.VisualScripting;
#endif
    public class PLOT_ChanceToEnableGO : PLOT
    {
        [SerializeField, Range(0, 1)]
        protected float chance = 0.5f;

        public override void Execute() => gameObject.SetActive(!SUtilities.Chance(chance * 100));
    }
    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT_ChanceToEnableGO))]
    public class PLOT_ChanceToEnableGOEditor : PLOTEditor { }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}





// ---------------------------------------------------------------------------------------------
