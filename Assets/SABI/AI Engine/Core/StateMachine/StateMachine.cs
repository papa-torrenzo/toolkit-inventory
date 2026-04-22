using UnityEngine;

namespace SABI
{
    [RequireComponent(typeof(NavmeshManager), typeof(AnimationManager))]
    public class StateMachine : BaseStateMachine
    {
        public NavmeshManager navMeshManager;
        public AnimationManager animationManager;

        protected virtual void Reset()
        {
            if (navMeshManager == null)
                navMeshManager = GetComponent<NavmeshManager>();
            if (animationManager == null)
                animationManager = GetComponent<AnimationManager>();
        }
    }
}
