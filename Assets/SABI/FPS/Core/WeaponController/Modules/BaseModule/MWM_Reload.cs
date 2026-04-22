using UnityEngine;

namespace SABI
{
    public abstract class MWM_Reload : MWM
    {
        [HideInInspector]
        public bool isReloading;
        public abstract void Reload();
    }
}
