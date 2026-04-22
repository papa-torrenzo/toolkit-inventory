using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Base/SageFloat")]
    public class SageFloat : BaseSageVariable<float>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CustomName = "Float Value SO";
        }

        public void Add(int valueToAdd = 1)
        {
            SetValue(GetValue() + valueToAdd);
        }

        public void Subtract(int valueToSubtract = 1)
        {
            SetValue(GetValue() - valueToSubtract);
        }
    }
}

#region Editor

#if UNITY_EDITOR

namespace SABI.SOA
{
    [CustomEditor(typeof(SageFloat), true)]
    public class SageFloatEditor : BaseSageVariableEditor { }
}
#endif

#endregion
