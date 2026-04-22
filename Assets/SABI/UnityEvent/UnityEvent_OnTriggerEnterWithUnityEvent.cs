namespace SABI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class UnityEvent_OnTriggerEnter : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent unityEvent;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnTriggerEnter(Collider other)
        {
            unityEvent.Invoke();

            if (debugMode)
                Debug.Log(
                    $"[UnityEvent_OnTriggerEnter : OnTriggerEnter ] from: {gameObject.name} TriggerWith: {other.gameObject}",
                    other.gameObject
                );
        }
    }
}
