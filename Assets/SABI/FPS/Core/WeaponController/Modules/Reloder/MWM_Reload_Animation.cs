using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Reload/MWM_Reload_Animation")]
    public class MWM_Reload_Animation : MWM_Reload
    {
        public float reloadTime = 1;
        public AudioClip audio_reload;
        public string animation_reload;

        public override void Reload()
        {
            if (isReloading)
                return;
            if (weapon.MWM_Magazine.GetIsMagazineFull())
                return;

            isReloading = true;
            if (weapon.animationManager)
                weapon.animationManager.SetAnimation(animationName: animation_reload);
            if (audio_reload)
                AudioManager.Instence.Play(audio_reload);
            Invoke(nameof(SetIsReloadingToFalse), reloadTime);
            weapon.MWM_SecondaryAimer.StopAiming();
        }

        private void SetIsReloadingToFalse()
        {
            weapon.MWM_Magazine.SetMagazineFull();
            isReloading = false;
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_Reload_Animation))]
    public class Editor_MWM_Reload_Animation : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_Reload_Animation)base.target;

            DrawDefaultInspector();

            if (target.audio_reload == null)
                EditorGUILayout.HelpBox(" Missing: audio_reload", MessageType.Warning);
            if (target.animation_reload.IsNullOrEmpty())
                EditorGUILayout.HelpBox(" Missing: animation_reload", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
