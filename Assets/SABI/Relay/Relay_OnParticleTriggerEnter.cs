namespace SABI
{
    using System;
    using UnityEngine;

    public class Relay_OnParticleTriggerEnter : MonoBehaviour
    {
        public Action OnParticleTriggerEnterFromRelay;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnParticleTrigger()
        {
            if (debugMode)
                Debug.Log($"[Relay_OnParticleTriggerEnter : OnParticleTrigger ]", gameObject);
            OnParticleTriggerEnterFromRelay?.Invoke();
        }
    }
}
