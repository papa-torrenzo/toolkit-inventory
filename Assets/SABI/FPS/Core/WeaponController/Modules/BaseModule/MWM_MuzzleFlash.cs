using UnityEngine;

namespace SABI
{
    public abstract class MWM_MuzzleFlash : MWM
    {
        public abstract void ShowMuzzleFlash();
        public abstract Vector3 GetMuzzleFlashPosition();
    }
}
