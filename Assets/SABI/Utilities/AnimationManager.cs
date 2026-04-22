using System.Collections.Generic;
using UnityEngine;

namespace SABI
{
    public class AnimationManager : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
        string currentAnimationName = "";

        void Awake()
        {
            if (!animator)
                animator = GetComponent<Animator>();
            if (!animator)
                animator = GetComponentInChildren<Animator>();
            if (!animator)
                Debug.LogError("No Animator Assigned or found on game object");
        }

        public void SetAnimation(
            string animationName = null,
            List<string> animationNameList = null,
            float transitionTime = 0.25f,
            bool canRepeatSameAnimation = false
        )
        {
            string animationToPlay = FindAnimationClipNameToPlay(animationName, animationNameList);

            if (currentAnimationName == animationToPlay)
            {
                if (!canRepeatSameAnimation)
                    return;
                else
                    animator.Play(animationToPlay, -1, 0);
            }

            animator.CrossFade(animationToPlay, transitionTime);

            currentAnimationName = animationToPlay;
        }

        private string FindAnimationClipNameToPlay(
            string animationName,
            List<string> animationNameList
        )
        {
            string animationToPlay = "";
            if (animationNameList != null && animationNameList.Count > 0)
            {
                animationToPlay = animationNameList[Random.Range(0, animationNameList.Count)];
            }
            else if (animationName != null)
            {
                animationToPlay = animationName;
            }
            else
            {
                Debug.LogError("No Animation Name Provided");
                return null;
            }

            return animationToPlay;
        }
    }
}
