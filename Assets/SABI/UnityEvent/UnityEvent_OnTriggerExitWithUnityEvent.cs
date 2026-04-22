namespace SABI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class UnityEvent_OnTriggerExit : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent unityEvent;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnTriggerExit(Collider other)
        {
            unityEvent.Invoke();

            if (debugMode)
                Debug.Log(
                    $"[UnityEvent_OnTriggerExit : OnTriggerExit ]  from: {gameObject.name} TriggerWith: {other.gameObject}",
                    other.gameObject
                );
        }
    }
}
