namespace SABI
{
    using System.Collections.Generic;
    using SABI;
    using SABI.Flow;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    public class SLinearPath : MonoBehaviour
    {
        [SerializeField]
        private bool visualizePath = true;

        public static List<SLinearPath> AllPatrolPoints = new();

        [SerializeField]
        private List<Transform> patrolPointsList = new();

        [SerializeField]
        private bool useRandomDeviationRange = true;

        [SerializeField, ShowIf(nameof(useRandomDeviationRange), true)]
        private float maxRandomDeviation = 1f;

        public Vector3 GetPosition(int index)
        {
            Vector3 posToReturn = patrolPointsList[index].position;
            if (useRandomDeviationRange)
            {
                Vector3 deviation = new Vector3(
                    Random.Range(-maxRandomDeviation, maxRandomDeviation),
                    0,
                    Random.Range(-maxRandomDeviation, maxRandomDeviation)
                );

                posToReturn += deviation;
            }
            return posToReturn;
        }

        void OnEnable() => AllPatrolPoints.Add(this);

        void OnDisable() => AllPatrolPoints.Remove(this);

        public int GetMaxPosition() => patrolPointsList.Count;

        public void AddNewPoint(Transform newPoint) => patrolPointsList.Add(newPoint);

        public void Clear()
        {
            foreach (var item in patrolPointsList)
                DestroyImmediate(item.gameObject);
            patrolPointsList.Clear();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!visualizePath)
                return;
            if (patrolPointsList == null || patrolPointsList.Count < 2)
                return;

            Gizmos.color = Color.green;

            for (int i = 0; i < patrolPointsList.Count; i++)
            {
                Transform current = patrolPointsList[i];
                Transform next = patrolPointsList[(i + 1) % patrolPointsList.Count]; // wrap to first

                if (current != null && next != null)
                {
                    // Draw line between points
                    Gizmos.DrawLine(current.position, next.position);

                    // Draw index number slightly above the point
                    Vector3 labelPos = current.position + Vector3.up * 0.3f;
                    Handles.color = Color.white;
                    Handles.Label(labelPos, i.ToString());

                    if (useRandomDeviationRange)
                        Gizmos.DrawWireSphere(current.position, maxRandomDeviation);
                }
            }
        }
#endif
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(SLinearPath))]
    public class SLinearPathEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            SLinearPath patrolPoints = target as SLinearPath;

            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            new Button() { text = "New Point" }
                .OnClick(() =>
                {
                    SerializedProperty patrolPointsProp = serializedObject.FindProperty(
                        "patrolPointsList"
                    );
                    SerializedProperty currentPointProp = serializedObject.FindProperty(
                        "currentPoint"
                    );
                    int patrolPointsSize = patrolPointsProp.arraySize;
                    Transform lastPatrolPoint =
                        patrolPointsSize != 0
                            ? patrolPointsProp
                                .GetArrayElementAtIndex(patrolPointsSize - 1)
                                .objectReferenceValue as Transform
                            : null;
                    // ---------------------------------------------------------------------------------------------
                    GameObject spawnedObject = new GameObject(
                        "Patrol Point " + (patrolPointsSize + 1)
                    );
                    patrolPoints.AddNewPoint(spawnedObject.transform);
                    spawnedObject.transform.parent = patrolPoints.transform;
                    // ---------------------------------------------------------------------------------------------
                    if (patrolPointsSize >= 1)
                        spawnedObject.transform.position = lastPatrolPoint.position;
                    else
                        spawnedObject.transform.localPosition = Vector3.zero;
                    // ---------------------------------------------------------------------------------------------
                    Selection.activeGameObject = spawnedObject;
                })
                .SetParent(root);
            new Button(() => patrolPoints.Clear()) { text = "Clear" }.SetParent(root);
            return root;
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
