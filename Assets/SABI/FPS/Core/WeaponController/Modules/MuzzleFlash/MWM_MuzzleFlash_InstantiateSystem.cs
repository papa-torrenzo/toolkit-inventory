using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/MuzzleFlash/MWM_MuzzleFlash_InstantiateSystem")]
    public class MWM_MuzzleFlash_InstantiateSystem : MWM_MuzzleFlash
    {
        [field: SerializeField]
        public GameObject muzzleFlashToInstantiate { get; private set; }

        [field: SerializeField]
        public Transform firePoint { get; private set; }

        [field: SerializeField]
        public bool autoDestroy { get; private set; } = true;

        public enum AutoDestroyModes
        {
            AutoDestroyAfterFixedTime,
            AutoDestroyAfterParticleSystemIsOver,
        }

        [HideInInspector, SerializeField]
        public AutoDestroyModes autoDestroyMode =
            AutoDestroyModes.AutoDestroyAfterParticleSystemIsOver;

        [HideInInspector, SerializeField]
        public ParticleSystem muzzleFlashParticleSystem;

        [HideInInspector, SerializeField]
        private float fixedDurationToDistroyParticleSystem = 0.5f;

        public override Vector3 GetMuzzleFlashPosition() =>
            firePoint?.transform?.position ?? transform.position;

        public override void ShowMuzzleFlash()
        {
            GameObject spawnedObject = Instantiate(
                muzzleFlashToInstantiate,
                firePoint.position,
                firePoint.rotation
            );

            if (!autoDestroy)
                return;
            float duration = autoDestroyMode switch
            {
                AutoDestroyModes.AutoDestroyAfterFixedTime => fixedDurationToDistroyParticleSystem,
                AutoDestroyModes.AutoDestroyAfterParticleSystemIsOver => muzzleFlashParticleSystem
                    .main
                    .duration,
                _ => 0.5f,
            };
            this.DelayedExecution(duration, () => spawnedObject.DestroyGameObject());
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_MuzzleFlash_InstantiateSystem))]
    public class Editor_MWM_MuzzleFlash_InstantiateSystem : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_MuzzleFlash_InstantiateSystem)base.target;

            DrawDefaultInspector();

            if (!target.autoDestroy)
                return;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoDestroyMode"));
            if (
                target.autoDestroyMode
                == MWM_MuzzleFlash_InstantiateSystem
                    .AutoDestroyModes
                    .AutoDestroyAfterParticleSystemIsOver
            )
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("muzzleFlashParticleSystem")
                );
            }
            else if (
                target.autoDestroyMode
                == MWM_MuzzleFlash_InstantiateSystem.AutoDestroyModes.AutoDestroyAfterFixedTime
            )
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("fixedDurationToDistroyParticleSystem")
                );
            }

            EditorGUILayout.Space(5);

            if (target.muzzleFlashToInstantiate == null)
                EditorGUILayout.HelpBox(" Missing: muzzleFlashToInstantiate", MessageType.Error);
            if (target.firePoint == null)
                EditorGUILayout.HelpBox(" Missing: firePoint", MessageType.Error);
            if (
                target.autoDestroy
                && target.autoDestroyMode
                    == MWM_MuzzleFlash_InstantiateSystem
                        .AutoDestroyModes
                        .AutoDestroyAfterParticleSystemIsOver
                && target.muzzleFlashParticleSystem == null
            )
                EditorGUILayout.HelpBox(" Missing: muzzleFlashParticleSystem", MessageType.Error);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
