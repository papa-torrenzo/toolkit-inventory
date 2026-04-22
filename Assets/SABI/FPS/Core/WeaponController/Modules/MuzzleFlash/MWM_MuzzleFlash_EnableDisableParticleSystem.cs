using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/MuzzleFlash/MWM_MuzzleFlash_EnableDisableParticleSystem")]
    public class MWM_MuzzleFlash_EnableDisableParticleSystem : MWM_MuzzleFlash
    {
        public enum Modes
        {
            DisableAfterParticleSystemIsOver,
            DisabledAfterFixedTime,
        }

        [field: SerializeField]
        public Modes mode { get; private set; } = Modes.DisableAfterParticleSystemIsOver;

        [HideInInspector, SerializeField]
        public ParticleSystem muzzleFlashParticleSystem;

        [HideInInspector, SerializeField]
        private float fixedDurationToDisableParticleSystem = 0.5f;

        [SerializeField]
        private GameObject gameObjectToEnableAndDisable;

        private void Awake() => gameObjectToEnableAndDisable.SetActive(false);

        public override Vector3 GetMuzzleFlashPosition() =>
            mode == Modes.DisableAfterParticleSystemIsOver
                ? muzzleFlashParticleSystem.transform.position
                : gameObjectToEnableAndDisable.transform.position;

        public override void ShowMuzzleFlash()
        {
            gameObjectToEnableAndDisable.SetActive(true);
            float duration = mode switch
            {
                Modes.DisabledAfterFixedTime => fixedDurationToDisableParticleSystem,
                Modes.DisableAfterParticleSystemIsOver => muzzleFlashParticleSystem.main.duration,
                _ => 0.5f,
            };
            this.DelayedExecution(duration, () => gameObjectToEnableAndDisable.SetActive(false));
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_MuzzleFlash_EnableDisableParticleSystem))]
    public class Editor_MWM_MuzzleFlash_EnableDisableParticleSystem : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_MuzzleFlash_EnableDisableParticleSystem)base.target;

            DrawDefaultInspector();

            if (
                target.mode
                == MWM_MuzzleFlash_EnableDisableParticleSystem
                    .Modes
                    .DisableAfterParticleSystemIsOver
            )
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("muzzleFlashParticleSystem")
                );
            }
            else if (
                target.mode
                == MWM_MuzzleFlash_EnableDisableParticleSystem.Modes.DisabledAfterFixedTime
            )
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("fixedDurationToDisableParticleSystem")
                );
            }

            EditorGUILayout.Space(5);

            if (
                target.muzzleFlashParticleSystem == null
                && target.mode
                    == MWM_MuzzleFlash_EnableDisableParticleSystem
                        .Modes
                        .DisableAfterParticleSystemIsOver
            )
            {
                EditorGUILayout.HelpBox(" Missing: muzzleFlashParticleSystem", MessageType.Error);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
