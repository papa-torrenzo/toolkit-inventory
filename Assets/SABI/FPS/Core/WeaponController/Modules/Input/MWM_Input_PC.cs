using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Input/MWM_Input_PC")]
    public class MWM_Input_PC : MWM_Input
    {
        private void Update()
        {
            if (weapon.MWM_Trigger.allowButtonHold)
            {
                if (Mouse.current.leftButton.isPressed)
                    weapon.MWM_Trigger.Trigger();
            }
            else
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                    weapon.MWM_Trigger.Trigger();
            }
            if (Keyboard.current.rKey.wasPressedThisFrame)
                weapon.MWM_Reload.Reload();
            if (Mouse.current.rightButton.wasPressedThisFrame)
                weapon.MWM_SecondaryAimer.StartAiming();
            if (Mouse.current.rightButton.wasReleasedThisFrame)
                weapon.MWM_SecondaryAimer.StopAiming();
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_Input_PC))]
    public class Editor_MWM_Input_PC : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_Input_PC)base.target;

            DrawDefaultInspector();

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleFlashParticleSystem"));


            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
