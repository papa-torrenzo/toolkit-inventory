using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/Gun Module/Ammo/MWM_Magazine_RechargingMagazine")]
    public class MWM_Magazine_RechargingMagazine : MWM_Magazine
    {
        public int maxMagazineSize = 100;
        public float rechargeSpeed = 1f;
        private int bulletsLeft;

        float rechargedAmount = 0;

        [field: SerializeField]
        public TextMeshProUGUI text { get; private set; }

        private void Start()
        {
            bulletsLeft = maxMagazineSize;
            UpdateUI();
        }

        public override bool GetIsMagazineFull() => bulletsLeft == maxMagazineSize;

        public override bool GetIsMagazineEmpty() => bulletsLeft == 0;

        public override int GetBulletsLeft() => bulletsLeft;

        public override void SetMagazineFull() => SetBulletsLeft(maxMagazineSize);

        public override void SetBulletsLeft(int bulletsLeft)
        {
            this.bulletsLeft = bulletsLeft;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (text)
                text.text = $"{bulletsLeft} / {maxMagazineSize}";
        }

        public override void RemoveOneBullet() => SetBulletsLeft(GetBulletsLeft() - 1);

        void Update()
        {
            rechargedAmount += Time.deltaTime * rechargeSpeed;
            if (rechargedAmount >= 1)
            {
                rechargedAmount = 0;
                SetBulletsLeft(GetBulletsLeft() + 1);
            }
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(MWM_Magazine_RechargingMagazine))]
    public class Editor_MWM_Magazine_RechargingMagazine : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = (MWM_Magazine_RechargingMagazine)base.target;

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
