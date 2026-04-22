namespace SABI
{
    using System.Collections.Generic;
    using SABI;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    public class PLOT_RandomMaterial : PLOT
    {
        [SerializeField]
        private List<MeshRenderer> meshRenderers;

        [SerializeField]
        private List<Material> materials;

        public override void Execute()
        {
            Material material = materials.GetRandomItem();
            meshRenderers.ForEach(item =>
            {
                item.material = material;
            });
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT_RandomMaterial))]
    public class PLOT_RandomMaterialEditor : PLOTEditor { }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
