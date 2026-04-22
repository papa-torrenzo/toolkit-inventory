using UnityEngine;

namespace SABI
{
    public abstract class MWM_SecondaryAimer : MWM
    {
        [HideInInspector]
        public bool isAiming;

        [Range(0f, 1f)]
        public float spreadMultiplayer = 0.25f;

        public abstract void StartAiming();
        public abstract void StopAiming();
    }
}
