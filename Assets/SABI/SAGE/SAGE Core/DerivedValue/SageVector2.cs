using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Base/Vector2ValueSO")]
    public class SageVector2 : BaseSageVariable<Vector2>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomName = "Vector2 Value SO";
        }
    }
}

#region Editor

#if UNITY_EDITOR

namespace SABI.SOA
{
    [CustomEditor(typeof(SageVector2), true)]
    public class SageVector2Editor : BaseSageVariableEditor { }
}
#endif

#endregion