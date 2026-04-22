using UnityEngine;

namespace SABI
{
    public abstract class MWM_BulletType : MWM
    {
        public float damage = 1;
        public abstract void BulletHit(Transform hitTransform, Vector3 hitPoint);
    }
}
