using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    public class MWM : MonoBehaviour
    {
        protected ModularWeapon weapon;

        public void SetGun(ModularWeapon weapon) => this.weapon = weapon;
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM))]
    public class Editor_MWM : Editor
    {
        protected void ValidateNullWithError(Object obj)
        {
            if (obj == null)
                EditorGUILayout.HelpBox($" {obj.name}  reference is missing!", MessageType.Error);
        }

        protected void ValidateNullWithWarning(Object obj)
        {
            if (obj == null)
                EditorGUILayout.HelpBox($" {obj.name}  reference is missing!", MessageType.Warning);
        }

        protected void ValidateNullWithInfo(Object obj)
        {
            if (obj == null)
                EditorGUILayout.HelpBox($" {obj.name} reference is missing!", MessageType.Info);
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
