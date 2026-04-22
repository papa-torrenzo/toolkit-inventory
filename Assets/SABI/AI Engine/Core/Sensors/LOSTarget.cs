namespace SABI
{
    using UnityEngine;

    public class LOSTarget : MonoBehaviour
    {
        [SerializeField]
        private string losTargetTag;

        private void Start()
        {
            if (losTargetTag == "")
                Debug.LogError("losTargetTag cant be empty", this);
        }

        public string GetLosTargetTag() => losTargetTag;
    }
}
