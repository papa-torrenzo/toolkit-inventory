using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SABI
{
    public abstract class State_Base : MonoBehaviour
    {
        [HideInInspector]
        public BaseStateMachine baseStateMachine;
        public List<Transition_Base> transitions;

        [HideInInspector]
        public List<Transition_Base> commonTransitions;

        [HideInInspector]
        public bool IsActive = false;

        [SerializeField]
        private BaseStateUnityEvents baseStateUnityEvents;

        public virtual bool Validation(out string validationMessage)
        {
            validationMessage = "";
            return true;
        }

        public virtual void StateEnter()
        {
            IsActive = true;
            // if (transitions.Count == 0)
            //     Debug.LogError("No Transition for state");
            TransitionsInit();
            TransitionsEnter();
            baseStateUnityEvents.OnStateEnter.Invoke();
        }

        public virtual void StateUpdate()
        {
            TransitionsUpdate();
            baseStateUnityEvents.OnStateUpdate.Invoke();
        }

        public virtual void StateExit()
        {
            TransitionsExit();
            commonTransitions.Clear();
            IsActive = false;
            baseStateUnityEvents.OnStateExit.Invoke();
        }

        public void SetCommonTransitions(List<Transition_Base> commonTransitions)
        {
            commonTransitions.ForEach(element => commonTransitions.Add(element));
        }

        #region Transition
        public void TransitionsInit()
        {
            transitions.ForEach(transition => transition.stateMachine = baseStateMachine);
            commonTransitions.ForEach(transition => transition.stateMachine = baseStateMachine);
        }

        public void TransitionsEnter()
        {
            transitions.ForEach(transition => transition.TransitionEnter());
            commonTransitions.ForEach(transition => transition.TransitionEnter());
        }

        public void TransitionsUpdate()
        {
            transitions.ForEach(transition => transition.TransitionUpdate());
            commonTransitions.ForEach(transition => transition.TransitionUpdate());
        }

        public void TransitionsExit()
        {
            transitions.ForEach(transition => transition.TransitionExit());
            commonTransitions.ForEach(transition => transition.TransitionExit());
        }
        #endregion
    }

    [System.Serializable]
    public class BaseStateUnityEvents
    {
        public UnityEvent OnStateEnter;
        public UnityEvent OnStateUpdate;
        public UnityEvent OnStateExit;
    }
}
