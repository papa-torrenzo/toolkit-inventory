using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Base/SageVector3")]
    public class SageVector3 : BaseSageVariable<Vector3>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomName = "Vector3 Value SO";
        }
    }
}

#region Editor

#if UNITY_EDITOR


namespace SABI.SOA
{
    [CustomEditor(typeof(SageVector3), true)]
    public class SageVector3Editor : BaseSageVariableEditor { }
}
#endif

#endregion