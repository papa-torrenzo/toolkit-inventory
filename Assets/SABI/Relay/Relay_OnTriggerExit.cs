namespace SABI
{
    using System;
    using UnityEngine;

    public class Relay_OnTriggerExit : MonoBehaviour
    {
        public Action<Collider> OnTriggerExitFromRelay;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnTriggerExit(Collider other)
        {
            if (debugMode)
                Debug.Log(
                    $"[Relay_OnTriggerExit : OnTriggerExit ]  from: {gameObject.name} TriggerWith: {other.gameObject}",
                    other.gameObject
                );
            OnTriggerExitFromRelay?.Invoke(other);
        }
    }
}
