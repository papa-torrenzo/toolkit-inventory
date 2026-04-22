using UnityEngine;

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Ammo/MWM_Magazine_None")]
    public class MWM_Magazine_None : MWM_Magazine
    {
        public override bool GetIsMagazineFull() => true;

        public override bool GetIsMagazineEmpty() => false;

        public override int GetBulletsLeft() => 1;

        public override void SetMagazineFull() { }

        public override void SetBulletsLeft(int bulletsLeft) { }

        public override void RemoveOneBullet() { }
    }
}
