using UnityEngine;

namespace SABI
{
    public class Transition_Base : MonoBehaviour
    {
        [HideInInspector]
        public BaseStateMachine stateMachine;

        public virtual void TransitionEnter() { }

        public virtual void TransitionUpdate() { }

        public virtual void TransitionExit() { }
    }
}
