using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Base/SageString")]
    public class SageString : BaseSageVariable<string>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomName = "String Value SO";
        }
    }
}


#region Editor

#if UNITY_EDITOR


namespace SABI.SOA
{
    [CustomEditor(typeof(SageString), true)]
    public class SageStringEditor : BaseSageVariableEditor { }
}
#endif

#endregion