namespace SABI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class UnityEvent_OnDestroy : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent unityEvent;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnDestroy()
        {
            unityEvent.Invoke();

            if (debugMode)
                Debug.Log(
                    $"[UnityEvent_OnDestroy : OnDestroy ] from: {gameObject.name} ",
                    gameObject
                );
        }
    }
}
