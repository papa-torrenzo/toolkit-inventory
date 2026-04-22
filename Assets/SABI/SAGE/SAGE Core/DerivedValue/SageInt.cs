using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Base/SageInt")]
    public class SageInt : BaseSageVariable<int>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomName = "Int Value SO";
        }
        public void Add(int valueToAdd = 1) { SetValue(GetValue() + valueToAdd); }

        public void Subtract(int valueToSubtract = 1) { SetValue(GetValue() - valueToSubtract); }
    }
}


#region Editor

#if UNITY_EDITOR

namespace SABI.SOA
{
    [CustomEditor(typeof(SageInt), true)]
    public class SageIntEditor : BaseSageVariableEditor { }
}
#endif

#endregion