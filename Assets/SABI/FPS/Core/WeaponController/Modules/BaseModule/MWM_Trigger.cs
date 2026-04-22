using UnityEngine;

namespace SABI
{
    public abstract class MWM_Trigger : MWM
    {
        [field: SerializeField]
        public bool allowButtonHold { get; private set; } = true;

        [field: SerializeField]
        public float spread { get; private set; } = 0.02f;

        [field: SerializeField]
        public float timeBetweenShooting { get; private set; } = 0.05f;

        public abstract void Trigger();
    }
}
