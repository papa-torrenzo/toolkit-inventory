using UnityEngine;

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/BulletType/MWM_BulletType_None")]
    public class MWM_BulletType_None : MWM_BulletType
    {
        public override void BulletHit(Transform hitTransform, Vector3 hitPoint) { }
    }
}
