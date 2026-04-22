using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Input/MWM_Input_InputSystem")]
    public class MWM_Input_InputSystem : MWM_Input
    {
        [Header("Input Actions")]
        [SerializeField]
        private InputActionReference fireAction;

        [SerializeField]
        private InputActionReference reloadAction;

        [SerializeField]
        private InputActionReference aimAction;

        // ---------------------------------------------------------------------------------------------

        [SerializeField]
        private InputActionAsset inputActionAsset;

        [SerializeField]
        private InputAction inputAction;

        // Exposed events for other systems to subscribe to
        // public event Action OnFirePerformed;
        // public event Action OnReloadPerformed;
        // public event Action OnAimStarted;
        // public event Action OnAimCanceled;

        private Coroutine holdFireCoroutine;

        private void OnEnable()
        {
            if (fireAction != null && fireAction.action != null)
            {
                fireAction.action.started += Fire_Started;
                fireAction.action.performed += Fire_Performed;
                fireAction.action.canceled += Fire_Canceled;
                fireAction.action.Enable();
            }

            if (reloadAction != null && reloadAction.action != null)
            {
                reloadAction.action.performed += Reload_Performed;
                reloadAction.action.Enable();
            }

            if (aimAction != null && aimAction.action != null)
            {
                aimAction.action.started += Aim_Started;
                aimAction.action.canceled += Aim_Canceled;
                aimAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (fireAction != null && fireAction.action != null)
            {
                fireAction.action.started -= Fire_Started;
                fireAction.action.performed -= Fire_Performed;
                fireAction.action.canceled -= Fire_Canceled;
                fireAction.action.Disable();
            }

            if (reloadAction != null && reloadAction.action != null)
            {
                reloadAction.action.performed -= Reload_Performed;
                reloadAction.action.Disable();
            }

            if (aimAction != null && aimAction.action != null)
            {
                aimAction.action.started -= Aim_Started;
                aimAction.action.canceled -= Aim_Canceled;
                aimAction.action.Disable();
            }

            StopHoldFire();
        }

        private void Fire_Started(InputAction.CallbackContext ctx)
        {
            if (weapon == null)
            {
                weapon = GetComponentInChildren<ModularWeapon>();
            }

            if (weapon == null || weapon.MWM_Trigger == null)
                return;

            if (weapon.MWM_Trigger.allowButtonHold)
            {
                StartHoldFire();
            }
        }

        private void Fire_Performed(InputAction.CallbackContext ctx)
        {
            if (weapon == null)
            {
                weapon = GetComponentInChildren<ModularWeapon>();
            }

            if (weapon == null || weapon.MWM_Trigger == null)
                return;

            // If not allowing hold, a performed event should fire single shots
            if (!weapon.MWM_Trigger.allowButtonHold)
            {
                weapon.MWM_Trigger.Trigger();
                // OnFirePerformed?.Invoke();
            }
        }

        private void Fire_Canceled(InputAction.CallbackContext ctx)
        {
            if (weapon == null)
            {
                weapon = GetComponentInChildren<ModularWeapon>();
            }

            if (weapon == null || weapon.MWM_Trigger == null)
                return;

            if (weapon.MWM_Trigger.allowButtonHold)
            {
                StopHoldFire();
            }
        }

        private void StartHoldFire()
        {
            if (holdFireCoroutine != null)
                return;
            holdFireCoroutine = StartCoroutine(HoldFireRoutine());
        }

        private void StopHoldFire()
        {
            if (holdFireCoroutine != null)
            {
                StopCoroutine(holdFireCoroutine);
                holdFireCoroutine = null;
            }
        }

        private IEnumerator HoldFireRoutine()
        {
            if (weapon == null)
            {
                weapon = GetComponentInChildren<ModularWeapon>();
            }

            if (weapon == null || weapon.MWM_Trigger == null)
                yield break;

            // Trigger immediately on hold start
            while (true)
            {
                weapon.MWM_Trigger.Trigger();
                // OnFirePerformed?.Invoke();
                float interval = Mathf.Max(0.001f, weapon.MWM_Trigger.timeBetweenShooting);
                yield return new WaitForSeconds(interval);
            }
        }

        private void Reload_Performed(InputAction.CallbackContext ctx)
        {
            Debug.Log($"[SAB] Reload Performed");

            if (weapon == null)
            {
                weapon = GetComponentInChildren<ModularWeapon>();
            }

            if (weapon == null || weapon.MWM_Reload == null)
                return;

            Debug.Log($"[SAB] Reload Detecetd");

            weapon.MWM_Reload.Reload();
            // OnReloadPerformed?.Invoke();
        }

        private void Aim_Started(InputAction.CallbackContext ctx)
        {
            if (weapon == null)
            {
                weapon = GetComponentInChildren<ModularWeapon>();
            }

            if (weapon == null || weapon.MWM_SecondaryAimer == null)
                return;
            weapon.MWM_SecondaryAimer.StartAiming();
            // OnAimStarted?.Invoke();
        }

        private void Aim_Canceled(InputAction.CallbackContext ctx)
        {
            if (weapon == null)
            {
                weapon = GetComponentInChildren<ModularWeapon>();
            }

            if (weapon == null || weapon.MWM_SecondaryAimer == null)
                return;
            weapon.MWM_SecondaryAimer.StopAiming();
            // OnAimCanceled?.Invoke();
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_Input_InputSystem))]
    public class Editor_MWM_Input_InputSystem : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_Input_InputSystem)base.target;

            DrawDefaultInspector();

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleFlashParticleSystem"));


            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
