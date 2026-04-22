using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SABI
{
    public class State_AnimationAction : State_Base
    {
        [SerializeField]
        private bool useMultipleAnimations = false;

        [SerializeField, ShowIf(nameof(useMultipleAnimations), false)]
        private string animationName;

        [SerializeField, ShowIf(nameof(useMultipleAnimations), true)]
        private string[] animationNames;
        private AnimationManager animationManager;

        [SerializeField, ShowIf(nameof(useMultipleAnimations), true)]
        private float delayBeforeChangingAnimations = 5;
        float timeTillNextAnimationChange = 0;

        public override void StateEnter()
        {
            base.StateEnter();
            animationManager =
                baseStateMachine.gameObject.GetComponentInChildren<AnimationManager>();
            if (useMultipleAnimations)
                animationManager.SetAnimation(animationName);
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
            if (useMultipleAnimations && animationNames.Length >= 1)
            {
                if (timeTillNextAnimationChange <= 0)
                {
                    animationManager.SetAnimation(animationNames.GetRandomItem());
                    timeTillNextAnimationChange = delayBeforeChangingAnimations;
                }
                else
                {
                    timeTillNextAnimationChange -= Time.deltaTime;
                }
            }
        }
    }
}
