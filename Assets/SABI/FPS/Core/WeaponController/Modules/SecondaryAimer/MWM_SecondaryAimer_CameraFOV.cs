using UnityEngine;

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/SecondaryAimer/MWM_SecondaryAimer_CameraFOV")]
    public class MWM_SecondaryAimer_CameraFOV : MWM_SecondaryAimer
    {
        [SerializeField]
        private bool useCinemachineCameraa = false;
        float initialFov;

        [SerializeField]
        private float newFov = 40;

        void Start()
        {
            initialFov = weapon.fpsCamera.fieldOfView;
        }

        public override void StartAiming()
        {
            if (useCinemachineCameraa) { }
            else
            {
                weapon.fpsCamera.fieldOfView = newFov;
            }
        }

        public override void StopAiming()
        {
            if (useCinemachineCameraa) { }
            else
            {
                weapon.fpsCamera.fieldOfView = initialFov;
            }
        }
    }
}
