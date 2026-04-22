namespace SABI
{
    using System;
    using UnityEngine;

    public class Relay_OnParticleCollisionEnter : MonoBehaviour
    {
        public Action<GameObject> OnParticleCollisionEnterFromRelay;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnParticleCollision(GameObject other)
        {
            if (debugMode)
                Debug.Log(
                    $"[Relay_OnParticleCollisionEnter : OnParticleCollision ] from: {gameObject.name} CollidedWith: {other.gameObject}",
                    other.gameObject
                );
            OnParticleCollisionEnterFromRelay?.Invoke(other);
        }
    }
}
