namespace SABI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class UnityEvent_OnCollisionEnter : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent unityEvent;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnCollisionEnter(Collision other)
        {
            unityEvent.Invoke();

            if (debugMode)
                Debug.Log(
                    $"[UnityEvent_OnCollisionEnter : OnCollisionEnter ] from: {gameObject.name} CollidedWith: {other.gameObject}",
                    other.gameObject
                );
        }
    }
}
