namespace SABI
{
    using System;
    using UnityEngine;

    public class Relay_OnCollisionEnter : MonoBehaviour
    {
        public Action<Collision> OnCollisionEnterFromRelay;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnCollisionEnter(Collision other)
        {
            if (debugMode)
                Debug.Log(
                    $"[Relay_OnCollisionEnter : OnCollisionEnter ] from: {gameObject.name} CollidedWith: {other.gameObject}",
                    other.gameObject
                );
            OnCollisionEnterFromRelay?.Invoke(other);
        }
    }
}
