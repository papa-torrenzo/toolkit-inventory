using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/BulletType/MWM_BulletType_SimpleDamage")]
    public class MWM_BulletType_SimpleDamage : MWM_BulletType
    {
        public override void BulletHit(Transform hitTransform, Vector3 hitPoint)
        {
            if (hitTransform.TryGetComponent(out IDamagable idamagable))
            {
                idamagable.TakeDamage(weapon.MWM_BulletType.damage, weapon);
            }
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_BulletType_SimpleDamage))]
    public class Editor_MWM_BulletType_SimpleDamage : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_BulletType_SimpleDamage)base.target;

            DrawDefaultInspector();

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleFlashParticleSystem"));


            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
