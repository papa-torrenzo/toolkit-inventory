using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Base/SageBool")]
    public class SageBool : BaseSageVariable<bool>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomName = "Bool Value SO";
        }
    }
}

#region Editor

#if UNITY_EDITOR

namespace SABI.SOA
{
    [CustomEditor(typeof(SageBool), true)]
    public class SageBoolEditor : BaseSageVariableEditor { }
}
#endif

#endregion