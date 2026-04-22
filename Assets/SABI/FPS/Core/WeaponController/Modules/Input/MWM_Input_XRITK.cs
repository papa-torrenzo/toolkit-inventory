// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit.Interactables;
// using UnityEngine.XR.Interaction.Toolkit.Interactors;
// #if UNITY_EDITOR
// using UnityEditor;
// #endif

// namespace SABI
// {
//     [AddComponentMenu("SABI/Gun Module/Input/GunModule_Input_XRITK")]
//     public class GunModule_Input_XRITK : GunBaseModule_Input
//     {
//         [SerializeField]
//         private XRGrabInteractable interactable;

//         void Awake()
//         {
//             if (interactable == null)
//                 interactable = GetComponent<XRGrabInteractable>();
//         }

//         void Update()
//         {
//             if (interactable.isSelected)
//             {
//                 if (interactable.firstInteractorSelecting is XRBaseInputInteractor inputInteractor)
//                 {
//                     if (inputInteractor.activateInput.ReadValue() == 1)
//                     {
//                         gun.gunModule_Trigger.Trigger();
//                     }
//                 }
//             }
//         }
//     }

//     //     #region Editor ------------------------------------------------------------------------- <Reg: Editor>

//     // #if UNITY_EDITOR

//     //     [CustomEditor(typeof(GunModule_Input_MetaSDK))]
//     //     public class Editor_GunModule_Input_MetaSDK : Editor
//     //     {
//     //         public override void OnInspectorGUI()
//     //         {
//     //             var target = (GunModule_Input_MetaSDK)base.target;

//     //             DrawDefaultInspector();

//     //             //EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleFlashParticleSystem"));


//     //             serializedObject.ApplyModifiedProperties();
//     //         }
//     //     }

//     // #endif

//     //     #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
// }
