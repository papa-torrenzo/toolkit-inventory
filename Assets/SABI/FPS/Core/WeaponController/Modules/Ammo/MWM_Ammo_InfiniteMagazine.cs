using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Ammo/MWM_Magazine_InfiniteMagazine")]
    public class MWM_Magazine_InfiniteMagazine : MWM_Magazine
    {
        [field: SerializeField]
        public TextMeshProUGUI text { get; private set; }

        private void Start()
        {
            if (text)
                text.text = $"∞";
        }

        public override bool GetIsMagazineFull() => true;

        public override bool GetIsMagazineEmpty() => false;

        public override int GetBulletsLeft() => 999;

        public override void SetMagazineFull() { }

        public override void SetBulletsLeft(int bulletsLeft) { }

        public override void RemoveOneBullet() { }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_Magazine_InfiniteMagazine))]
    public class Editor_MWM_Magazine_InfiniteMagazine : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_Magazine_InfiniteMagazine)base.target;

            DrawDefaultInspector();

            EditorGUILayout.Space(5);

            if (target.text == null)
            {
                EditorGUILayout.HelpBox(" Missing: text", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
