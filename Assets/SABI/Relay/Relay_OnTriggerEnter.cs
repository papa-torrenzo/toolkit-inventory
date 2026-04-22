namespace SABI
{
    using System;
    using UnityEngine;

    public class Relay_OnTriggerEnter : MonoBehaviour
    {
        public Action<Collider> OnTriggerEnterFromRelay;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (debugMode)
                Debug.Log(
                    $"[Relay_OnTriggerEnter : OnTriggerEnter ] from: {gameObject.name} TriggerWith: {other.gameObject}",
                    other.gameObject
                );
            OnTriggerEnterFromRelay?.Invoke(other);
        }
    }
}
