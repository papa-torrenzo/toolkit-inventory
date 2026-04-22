using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    public abstract class MWM_Shooter_Simple : MWM_Shooter
    {
        [field: SerializeField]
        public AudioClip audio_shoot { get; private set; }

        [field: SerializeField]
        public AudioClip audio_dryFire { get; private set; }

        [field: SerializeField]
        public string animation_shoot { get; private set; }

        int individuelShotesLeftInSingleTrigger;

        [field: SerializeField]
        public float timeBetweenEachIndividuelShooting { get; private set; } = 0.1f;

        [field: SerializeField]
        public int maxIndividuelShotesPerSingleTrigger { get; private set; } = 1;

        [field: SerializeField]
        public int range { get; private set; } = 100;

        [field: SerializeField]
        public LayerMask raycastLayerMasak { get; private set; } = 1 << 0;

        [field: SerializeField]
        public bool shouldConsiderIndividuelShootAsOneBullet { get; private set; } = true;
        private List<LineRenderer> lineRenderers = new();

        [field: SerializeField]
        public Material lineRendererMaterial { get; private set; }

        [field: SerializeField]
        public Transform shellEjectPoint { get; private set; }

        bool canShoot = true;

        private void Awake()
        {
            for (int i = 0; i < maxIndividuelShotesPerSingleTrigger; i++)
            {
                GameObject lineRendereGameObject = new GameObject(
                    $"Line Rendere {i}",
                    typeof(LineRenderer)
                );
                lineRendereGameObject.transform.parent = transform;
                LineRenderer lineRenderer = lineRendereGameObject.GetComponent<LineRenderer>();
                lineRenderers.Add(lineRenderer);
                if (lineRendererMaterial)
                    lineRenderer.material = lineRendererMaterial;
                lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, 0.02f);
            }
        }

        public override void Shoot()
        {
            if (!canShoot)
                return;
            if (weapon.MWM_Reload.isReloading)
                return;
            if (weapon.MWM_Magazine.GetIsMagazineEmpty())
            {
                DryFire();
                return;
            }

            canShoot = false;
            individuelShotesLeftInSingleTrigger = maxIndividuelShotesPerSingleTrigger;
            SingleShoot();
        }

        private void DryFire()
        {
            if (audio_dryFire)
                AudioManager.Instence.Play(audio_dryFire);
        }

        protected virtual void SingleShoot()
        {
            // SUtilities.Log($"Single Shoot");
            // Recoile Module
            float spreadRange = weapon.MWM_Trigger.spread;

            if (weapon.MWM_SecondaryAimer.isAiming)
                spreadRange *= weapon.MWM_SecondaryAimer.spreadMultiplayer;

            float recoilX = Random.Range(-spreadRange, spreadRange);
            float recoilY = Random.Range(-spreadRange, spreadRange);

            Vector3 direction = weapon.firePoint.forward + new Vector3(recoilX, recoilY, 0);

            if (weapon.animationManager && animation_shoot.Trim() != "")
                weapon.animationManager.SetAnimation(
                    animationName: animation_shoot,
                    canRepeatSameAnimation: true,
                    transitionTime: 0.05f
                );

            individuelShotesLeftInSingleTrigger--;

            SingleShootFireLogic(direction);

            AudioMuzzleFlashAndBulletReduction();

            Invoke(nameof(SetCanShootToTrue), weapon.MWM_Trigger.timeBetweenShooting);
            if (
                individuelShotesLeftInSingleTrigger > 0
                && !weapon.MWM_Magazine.GetIsMagazineEmpty()
            )
                Invoke(nameof(SingleShoot), timeBetweenEachIndividuelShooting);
        }

        protected abstract void SingleShootFireLogic(Vector3 direction);

        protected void TrailByLineRendered(Vector3 endPoint)
        {
            LineRenderer lineRenderer = lineRenderers[individuelShotesLeftInSingleTrigger];
            lineRenderer.SetPositions(
                new Vector3[] { weapon.MWM_MuzzleFlash.GetMuzzleFlashPosition(), endPoint }
            );
            lineRenderer.enabled = true;
            this.DelayedExecution(0.005f, () => lineRenderer.enabled = false);
        }

        private void AudioMuzzleFlashAndBulletReduction()
        {
            if (
                maxIndividuelShotesPerSingleTrigger > 0
                && !shouldConsiderIndividuelShootAsOneBullet
                && individuelShotesLeftInSingleTrigger != 1
            )
                return;

            if (audio_shoot)
                AudioManager.Instence.Play(audio_shoot);
            weapon.MWM_MuzzleFlash.ShowMuzzleFlash();
            weapon.MWM_Magazine.RemoveOneBullet();
        }

        private void SetCanShootToTrue()
        {
            canShoot = true;
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_Shooter_Simple))]
    public class Editor_MWM_Shooter_Simple : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_Shooter_Simple)base.target;
            if (target.audio_shoot == null)
                EditorGUILayout.HelpBox(" Missing: audio_shoot", MessageType.Warning);
            if (target.audio_dryFire == null)
                EditorGUILayout.HelpBox(" Missing: audio_dryFire", MessageType.Warning);
            if (target.animation_shoot.IsNullOrEmpty())
                EditorGUILayout.HelpBox(" Missing: animation_shoot", MessageType.Warning);
            if (target.lineRendererMaterial == null)
                EditorGUILayout.HelpBox(" Missing: lineRendererMaterial", MessageType.Error);
            if (target.shellEjectPoint == null)
                EditorGUILayout.HelpBox(" Missing: shellEjectPoint", MessageType.Warning);
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
