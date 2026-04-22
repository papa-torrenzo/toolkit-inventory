namespace SABI
{
    using System;
    using UnityEngine;

    public class Relay_OnEnable : MonoBehaviour
    {
        public Action OnEnableFromRelay;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnEnable()
        {
            if (debugMode)
                Debug.Log($"[Relay_OnEnable : OnEnable ] from: {gameObject.name} ", gameObject);
            OnEnableFromRelay?.Invoke();
        }
    }
}
