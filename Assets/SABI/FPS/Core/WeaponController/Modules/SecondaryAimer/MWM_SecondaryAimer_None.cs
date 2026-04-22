using UnityEngine;

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/SecondaryAimer/MWM_SecondaryAimer_None")]
    public class MWM_SecondaryAimer_None : MWM_SecondaryAimer
    {
        public override void StartAiming() { }

        public override void StopAiming() { }
    }
}
