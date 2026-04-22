namespace SABI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class UnityEvent_Start : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent unityEvent;

        [SerializeField]
        protected bool debugMode;

        protected virtual void Start()
        {
            unityEvent.Invoke();

            if (debugMode)
                Debug.Log($"[UnityEvent_Start : Start ] from: {gameObject.name} ", gameObject);
        }
    }
}
