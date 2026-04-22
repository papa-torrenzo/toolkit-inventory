namespace SABI
{
    using SABI;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    public class PLOT_HandleChildPLOT : PLOT
    {
        public override void Execute()
        {
            PLOT[] plots = gameObject.GetComponentsInChildren<PLOT>(true);

            if (plots == null)
                return;

            plots.ForEach(item => item.gameObject.Enable());

            plots.ForEach(item =>
            {
                if (
                    item != null
                    && item != this
                    && item.gameObject != gameObject
                    && item is not PLOT_HandleChildPLOT
                    && item.gameObject.activeInHierarchy
                )
                    item.Execute();
            });
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT_HandleChildPLOT))]
    public class PLOT_HandleChildPLOTEditor : PLOTEditor
    {
        public override void CustomThings()
        {
            base.CustomThings();
            buttons.Add(
                "Enable All GO",
                () =>
                {
                    (target as PLOT_HandleChildPLOT)
                        .gameObject.GetComponentsInChildren<Transform>(true)
                        .ForEach(item =>
                        {
                            item.gameObject.Enable();
                        });
                }
            );
            buttons.Add(
                "Enable All GO With Plot",
                () =>
                {
                    (target as PLOT_HandleChildPLOT)
                        .gameObject.GetComponentsInChildren<PLOT>(true)
                        .ForEach(item =>
                        {
                            item.gameObject.Enable();
                        });
                }
            );
            buttons.Add(
                "Destroy All Deactivated GO With Plot",
                () =>
                {
                    bool canDo = EditorUtility.DisplayDialog(
                        "Destroy All Deactivated Plot's ",
                        "Do you want to destroy All Deactivated Gam object With Plot ?",
                        "Delete"
                    );
                    if (canDo)
                    {
                        (target as PLOT_HandleChildPLOT)
                            .gameObject.GetComponentsInChildren<PLOT>(true)
                            .ForEach(item =>
                            {
                                if (
                                    item != (target as PLOT)
                                    && !item.gameObject.activeInHierarchy
                                    && !item.gameObject.activeSelf
                                )
                                {
                                    GameObject.DestroyImmediate(item.gameObject);
                                }
                            });
                    }
                }
            );
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
