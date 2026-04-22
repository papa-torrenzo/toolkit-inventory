namespace SABI
{
    using System;
    using UnityEngine;

    public class Relay_OnDestroy : MonoBehaviour
    {
        public Action OnDestroyFromRelay;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnDestroy()
        {
            if (debugMode)
                Debug.Log($"[Relay_OnDestroy : OnDestroy ] from: {gameObject.name} ", gameObject);
            OnDestroyFromRelay?.Invoke();
        }
    }
}
