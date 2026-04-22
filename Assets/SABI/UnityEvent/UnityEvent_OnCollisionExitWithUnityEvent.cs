namespace SABI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class UnityEvent_OnCollisionExit : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent unityEvent;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnCollisionExit(Collision other)
        {
            unityEvent.Invoke();

            if (debugMode)
                Debug.Log(
                    $"[UnityEvent_OnCollisionExit : OnCollisionExit ] from: {gameObject.name} CollidedWith: {other.gameObject}",
                    other.gameObject
                );
        }
    }
}
