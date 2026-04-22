using UnityEngine;

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Trigger/MWM_Trigger_Automatic")]
    public class MWM_Trigger_Automatic : MWM_Trigger
    {
        int maxBulletsPerSingleFire = 1;

        public override void Trigger() => weapon.MWM_Shooter.Shoot();
    }
}
