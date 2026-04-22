using UnityEngine;

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SOA/Base/IntValueSO")]
    public class IntValueSO : BaseValueSO<int>
    {
        public void Add(int valueToAdd = 1) => SetValue(GetValue() + valueToAdd);
        public void Subtract(int valueToSubtract = 1) => SetValue(GetValue() - valueToSubtract);
    }
}