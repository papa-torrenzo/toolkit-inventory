using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Base/SageTransform")]
    public class SageTransform : BaseSageVariable<Transform>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomName = "Transform Value SO";
        }
    }
}

#region Editor

#if UNITY_EDITOR


namespace SABI.SOA
{
    [CustomEditor(typeof(SageTransform), true)]
    public class SageTransformEditor : BaseSageVariableEditor { }
}
#endif

#endregion