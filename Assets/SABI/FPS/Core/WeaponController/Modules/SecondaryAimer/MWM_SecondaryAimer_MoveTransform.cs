using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/SecondaryAimer/MWM_SecondaryAimer_MoveTransform")]
    public class GunModule_MWM_MoveTransform : MWM_SecondaryAimer
    {
        [field: SerializeField]
        public Transform gunTransform { get; private set; }

        [field: SerializeField]
        public Vector3 positionOnAiming { get; private set; }

        [field: SerializeField]
        public Vector3 rotationOnAiming { get; private set; }
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private float initialFieldOfView;

        [SerializeField, Range(0.1f, 1f)]
        private float fovMultiplayer = 0.75f;

        [field: SerializeField]
        public AudioClip audio_StartAiming { get; private set; }

        [field: SerializeField]
        public AudioClip audio_EndAiming { get; private set; }

        private float transitionDuration = 0.2f;

        private void Awake()
        {
            initialPosition = gunTransform.localPosition;
            initialRotation = gunTransform.localRotation;
        }

        private void Start()
        {
            initialFieldOfView = weapon.fpsCamera.fieldOfView;
        }

        public override void StartAiming()
        {
            if (isAiming)
                return;
            isAiming = true;
            StopAllCoroutines();
            StartCoroutine(
                SmoothTransition(
                    positionOnAiming,
                    rotationOnAiming,
                    initialFieldOfView * fovMultiplayer
                )
            );
            AudioManager.Instence.Play(audio_StartAiming);
        }

        public override void StopAiming()
        {
            if (!isAiming)
                return;
            isAiming = false;
            StopAllCoroutines();
            StartCoroutine(
                SmoothTransition(initialPosition, initialRotation.eulerAngles, initialFieldOfView)
            );
            AudioManager.Instence.Play(audio_EndAiming);
        }

        private IEnumerator SmoothTransition(
            Vector3 targetPosition,
            Vector3 targetRotation,
            float targetFieldOfView
        )
        {
            Vector3 startPosition = gunTransform.localPosition;
            Quaternion startRotation = gunTransform.localRotation;
            float startFieldOfView = weapon.fpsCamera.fieldOfView;
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;
                gunTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                gunTransform.localRotation = Quaternion.Slerp(
                    startRotation,
                    Quaternion.Euler(targetRotation),
                    t
                );
                weapon.fpsCamera.fieldOfView = Mathf.Lerp(startFieldOfView, targetFieldOfView, t);
                yield return null;
            }

            gunTransform.localPosition = targetPosition;
            gunTransform.localRotation = Quaternion.Euler(targetRotation);
            weapon.fpsCamera.fieldOfView = targetFieldOfView;
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(GunModule_MWM_MoveTransform))]
    public class Editor_MWM_SecondaryAimer_MoveTransform : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (GunModule_MWM_MoveTransform)base.target;

            DrawDefaultInspector();

            EditorGUILayout.Space(5);

            if (target.gunTransform == null)
                EditorGUILayout.HelpBox(" Missing: gunTransform", MessageType.Error);

            if (target.audio_StartAiming == null)
                EditorGUILayout.HelpBox(" Missing: audio_StartAiming", MessageType.Warning);
            if (target.audio_EndAiming == null)
                EditorGUILayout.HelpBox(" Missing: audio_EndAiming", MessageType.Warning);

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
