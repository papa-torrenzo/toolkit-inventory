using UnityEngine;

namespace SABI.SOA
{

    [CreateAssetMenu(menuName = "SOA/Resource/IntValueResourceSO")]
    public class IntValueResourceSO : IntValueSO
    {
        public bool TrySpending(int valueToSpend)
        {
            if (CanSpend(valueToSpend))
            {
                Subtract(valueToSpend);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanSpend(int valueToSpend) => valueToSpend >= GetValue();
    }
}