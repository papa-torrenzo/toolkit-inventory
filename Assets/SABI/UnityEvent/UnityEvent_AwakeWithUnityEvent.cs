namespace SABI
{
    using UnityEngine;
    using UnityEngine.Events;

    public class UnityEvent_Awake : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent unityEvent;

        [SerializeField]
        protected bool debugMode;

        protected virtual void Awake()
        {
            unityEvent.Invoke();

            if (debugMode)
                Debug.Log(
                    $"[UnityEvent_Awake : OnCollisionEnter ] from: {gameObject.name} ",
                    gameObject
                );
        }
    }
}
