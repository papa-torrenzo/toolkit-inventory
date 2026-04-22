using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Shooter/MWM_RaycastShooter")]
    public class MWM_RaycastShooter : MWM_Shooter_Simple
    {
        public GameObject bulletHole;

        protected override void SingleShootFireLogic(Vector3 direction)
        {
            RaycastHit raycastHit;
            if (
                Physics.Raycast(
                    weapon.firePoint.position,
                    direction,
                    out raycastHit,
                    range,
                    raycastLayerMasak,
                    QueryTriggerInteraction.Ignore
                )
            )
            {
                weapon.MWM_BulletType.BulletHit(raycastHit.collider.transform, raycastHit.point);
                TrailByLineRendered(raycastHit.point);
                if (bulletHole)
                    Instantiate(
                        bulletHole,
                        raycastHit.point,
                        Quaternion.LookRotation(raycastHit.normal)
                    );
            }
            else
            {
                TrailByLineRendered(weapon.fpsCamera.transform.position + (direction * range));
            }
        }

        #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

        [CustomEditor(typeof(MWM_RaycastShooter))]
        public class Editor_MWM_RaycastShooter : Editor_MWM_Shooter_Simple
        {
            public override void OnInspectorGUI()
            {
                var target = (MWM_RaycastShooter)base.target;

                DrawDefaultInspector();

                EditorGUILayout.Space(5);

                if (target.bulletHole == null)
                    EditorGUILayout.HelpBox(" Missing: bulletHole", MessageType.Warning);

                base.OnInspectorGUI();
            }
        }

#endif

        #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
    }
}
