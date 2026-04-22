using UnityEngine;

namespace SABI.SOA
{
    [CreateAssetMenu(menuName = "SAGE/Resource/IntValueResourceSO")]
    public class SageIntValueResourceSo : SageInt
    {
        public bool TrySpending(int valueToSpend)
        {
            if (CanSpend(valueToSpend))
            {
                Subtract(valueToSpend);
                return true;
            }

            return false;
        }

        public bool CanSpend(int valueToSpend) { return valueToSpend >= GetValue(); }
    }
}