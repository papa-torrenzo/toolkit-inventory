using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Base/SageGameObject")]
    public class SageGameObject : BaseSageVariable<GameObject>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomName = "GameObject Value SO";
        }
    }
}

#region Editor

#if UNITY_EDITOR

namespace SABI.SOA
{
    [CustomEditor(typeof(SageGameObject), true)]
    public class SageGameObjectEditor : BaseSageVariableEditor { }
}
#endif

#endregion