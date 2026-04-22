namespace SABI
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class DebugStatusSystem : MonoBehaviour
    {
        [SerializeField]
        private List<StatusElementType> statusElementTypes;

        [SerializeField]
        private bool active = true,
            showAll = true;
        StatusSystem statusSystem;

        void Awake()
        {
            statusSystem = GetComponent<StatusSystem>();
        }

        void OnDrawGizmos()
        {
            // return;

            if (!active)
                return;

            if (statusSystem == null)
                return;

            if (showAll)
            {
                int index = 1;
                foreach (var item in statusSystem.StatusDictionary)
                {
                    DebugStatusElementType(item.Key, index);
                    index++;
                }
            }
            else if (statusElementTypes.Count > 0)
            {
                int index = 1;
                foreach (var item in statusElementTypes)
                {
                    DebugStatusElementType(item, index);
                    index++;
                }
            }
        }

        private void DebugStatusElementType(StatusElementType statusElementType, int index)
        {
            StatusData data = statusSystem.StatusDictionary[statusElementType];
            float progress = Mathf.Clamp01(data.CurrentValue / data.MaxValue);

            Vector3 center = transform.position + Vector3.up * 2 + Vector3.up * index * 0.2f;
            float fullWidth = 2.0f;

            // 1. Draw the Label to the left of the bar
            // We offset the text position by half the width plus a small padding
            Vector3 labelPos = center - new Vector3((fullWidth / 2) + 0.2f, 0, 0);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            // This adds a semi-transparent dark rounded box behind the text
            style.normal.background = Texture2D.blackTexture;
            style.alignment = TextAnchor.MiddleRight;
            style.padding = new RectOffset(4, 4, 2, 2); // Add some breathing room
#if UNITY_EDITOR
            Handles.Label(labelPos, statusElementType.ToString(), style);
#endif

            // 2. Draw the Background (Wireframe)
            Gizmos.color = Color.gray;
            Gizmos.DrawCube(center, new Vector3(fullWidth, 0.1f, 0.1f));

            // 3. Draw the Fill
            Gizmos.color = Color.blue;
            Vector3 barScale = new Vector3(fullWidth * progress, 0.1f, 0.1f);
            Vector3 barPos = center - new Vector3((fullWidth * (1 - progress)) / 2, 0, 0);

            Gizmos.DrawCube(barPos, barScale);
        }
    }
}
