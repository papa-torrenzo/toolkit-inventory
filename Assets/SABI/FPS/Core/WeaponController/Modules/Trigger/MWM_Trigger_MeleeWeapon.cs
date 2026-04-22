using UnityEngine;

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Trigger/MWM_Trigger_MeleeWeapon")]
    public class MWM_Trigger_MeleeWeapon : MWM_Trigger
    {
        [SerializeField]
        private string animationStateName;

        [SerializeField]
        private bool canAttack = true;

        [SerializeField]
        private float delayBtwAttacks = 1;

        // float timeLeftForNextAttack = 0;

        public override void Trigger()
        {
            if (canAttack)
            {
                canAttack = false;
                weapon.animationManager.SetAnimation(animationStateName);
            }
            else
            {
                this.DelayedExecution(
                    delayBtwAttacks,
                    () =>
                    {
                        canAttack = true;
                    }
                );
            }
        }
    }
}
