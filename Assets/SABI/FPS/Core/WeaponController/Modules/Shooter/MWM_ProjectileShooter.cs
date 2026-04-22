using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Shooter/MWM_ProjectileShooter")]
    public class MWM_ProjectileShooter : MWM_Shooter_Simple
    {
        [field: SerializeField]
        public BaseProjectile projectile { get; private set; }

        [field: SerializeField]
        public Transform firePoint { get; private set; }

        protected override void SingleShootFireLogic(Vector3 direction)
        {
            BaseProjectile spawnedProjectile = Instantiate(
                projectile,
                firePoint.position,
                firePoint.rotation
            );
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_ProjectileShooter))]
    public class Editor_MWM_ProjectileShooter : Editor_MWM_Shooter_Simple
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_ProjectileShooter)base.target;

            DrawDefaultInspector();

            EditorGUILayout.Space(5);

            if (target.projectile == null)
                EditorGUILayout.HelpBox(" Missing: projectile", MessageType.Error);

            if (target.firePoint == null)
                EditorGUILayout.HelpBox(" Missing: firePoint", MessageType.Error);

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
