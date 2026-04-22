using System;
using System.Collections.Generic;
using UnityEngine;

namespace SABI
{
    public class Transition_OnSenseSomething : Transition_Base
    {
        Sensor_Range sensor_Range;

        [SerializeField]
        private List<TransitionToStateBasedOnLODTargetTag> transitionToStateBasedOnLODTargetTags;

        public override void TransitionEnter()
        {
            base.TransitionEnter();
            sensor_Range = stateMachine.GetComponentInChildren<Sensor_Range>();
            sensor_Range.OnTargetDetectionChange += OnTargetDetected;
            Validation();
        }

        public override void TransitionExit()
        {
            base.TransitionExit();
            sensor_Range.OnTargetDetectionChange -= OnTargetDetected;
        }

        protected virtual void Validation()
        {
            foreach (var item in transitionToStateBasedOnLODTargetTags)
            {
                if (item.lodTargetTag == "")
                    Debug.LogError(item.lodTargetTag = "", this);
                if (item.stateToTransition_OnVisanEnter == null)
                    Debug.LogError(item.stateToTransition_OnVisanEnter = null, this);
            }
        }

        protected virtual void OnTargetDetected(
            GameObject target,
            string losTargetTag,
            bool isVisible
        )
        {
            foreach (var item in transitionToStateBasedOnLODTargetTags)
            {
                if (item.lodTargetTag == losTargetTag)
                {
                    if (item.stateToTransition_OnVisanEnter is State_ChaseTarget)
                    {
                        (item.stateToTransition_OnVisanEnter as State_ChaseTarget).Init(
                            target.transform
                        );
                    }

                    stateMachine.SetState(
                        isVisible
                            ? item.stateToTransition_OnVisanEnter
                            : item.stateToTransition_OnVisanExit
                    );
                }
            }
        }
    }
}
