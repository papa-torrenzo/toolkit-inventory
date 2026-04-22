// using UnityEngine;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif

// namespace SABI
// {
//     [AddComponentMenu("SABI/Gun Module/Input/MWM_Input_MetaSDK")]
//     public class GunModule_Input_MetaSDK : GunBaseModule_Input
//     {
//         [SerializeField]
//         private OVRInput.RawButton input_shoot = OVRInput.RawButton.RIndexTrigger,
//             // input_seconderyAim,
//             input_reload = OVRInput.RawButton.A;

//         private void Update()
//         {
//             if (gun.gunModule_Trigger.allowButtonHold)
//             {
//                 if (OVRInput.Get(input_shoot))
//                     gun.gunModule_Trigger.Trigger();
//             }
//             else
//             {
//                 if (OVRInput.GetDown(input_shoot))
//                     gun.gunModule_Trigger.Trigger();
//             }
//             if (OVRInput.GetDown(input_reload))
//                 gun.gunModule_Reload.Reload();
//         }
//     }

//     #region Editor ------------------------------------------------------------------------- <Reg: Editor>

// #if UNITY_EDITOR

//     [CustomEditor(typeof(GunModule_Input_MetaSDK))]
//     public class Editor_GunModule_Input_MetaSDK : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             var target = (GunModule_Input_MetaSDK)base.target;

//             DrawDefaultInspector();


//             //EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleFlashParticleSystem"));


//             serializedObject.ApplyModifiedProperties();
//         }
//     }

// #endif

//     #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
// }
