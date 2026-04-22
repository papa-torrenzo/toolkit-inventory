namespace SABI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class UnityEvent_OnDisable : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent unityEvent;

        [SerializeField]
        protected bool debugMode;

        protected virtual void OnDisable()
        {
            unityEvent.Invoke();

            if (debugMode)
                Debug.Log(
                    $"[UnityEvent_OnDisable : OnDisable ] from: {gameObject.name} ",
                    gameObject
                );
        }
    }
}
