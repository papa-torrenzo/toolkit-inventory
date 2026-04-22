using UnityEngine;

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/MuzzleFlash/MWM_MuzzleFlash_None")]
    public class MWM_MuzzleFlash_None : MWM_MuzzleFlash
    {
        public override void ShowMuzzleFlash() { }

        public override Vector3 GetMuzzleFlashPosition()
        {
            if (weapon.firePoint)
            {
                return weapon.firePoint.transform.position;
            }
            else if (weapon.fpsCamera)
            {
                return weapon.fpsCamera.transform.position;
            }
            else
            {
                return transform.position;
            }
        }
    }
}
