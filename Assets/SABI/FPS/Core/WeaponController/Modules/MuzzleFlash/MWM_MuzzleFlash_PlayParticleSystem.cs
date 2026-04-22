using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/MuzzleFlash/MWM_MuzzleFlash_PlayParticleSystem")]
    public class GunModule_MWM_PlayParticleSystem : MWM_MuzzleFlash
    {
        [field: SerializeField]
        public ParticleSystem muzzleFlash { get; private set; }

        public override Vector3 GetMuzzleFlashPosition() =>
            muzzleFlash?.transform?.position ?? transform.position;

        public override void ShowMuzzleFlash()
        {
            if (muzzleFlash)
                muzzleFlash.Play();
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(GunModule_MWM_PlayParticleSystem))]
    public class Editor_MWM_MuzzleFlash_PlayParticleSystem : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (GunModule_MWM_PlayParticleSystem)base.target;

            DrawDefaultInspector();

            EditorGUILayout.Space(5);

            if (target.muzzleFlash == null)
                EditorGUILayout.HelpBox(" Missing: muzzleFlash", MessageType.Error);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
