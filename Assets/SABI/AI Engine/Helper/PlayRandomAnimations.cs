using SABI;
using UnityEngine;

namespace SABI
{
    public class PlayRandomAnimations : MonoBehaviour
    {
        [SerializeField]
        private string[] animationsStates;

        [SerializeField]
        private Vector2 delayRange = new Vector2(3, 5);
        Animator animator;
        float timeLeft = 3;

        void Awake()
        {
            animator = GetComponent<Animator>();
            timeLeft = Random.Range(delayRange.x, delayRange.y);
        }

        void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = Random.Range(delayRange.x, delayRange.y);
                animator.CrossFade(animationsStates.GetRandomItem(), 0.2f);
            }
        }
    }
}
