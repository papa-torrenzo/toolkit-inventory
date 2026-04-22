using SABI;
using UnityEngine;

namespace SABI
{
    public class EnableRandomGameObject : MonoBehaviour
    {
        [Button]
        void Start()
        {
            int childToEnable = Random.Range(0, transform.childCount);
            for (int i = 0; i < transform.childCount; i++)
            {
                if (childToEnable == i)
                {
                    transform.GetChild(i).gameObject.SetActive(childToEnable == i);
                }
                else
                {
                    transform.GetChild(i).gameObject.DestroyGameObject();
                }
            }
        }
    }
}
