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

    public class SPath : MonoBehaviour
    {
        [SerializeField]
        private bool visualizePath = true;

        public static List<SPath> AllPatrolPoints = new();

        public List<SPathPoint> pathPoints = new();

        [SerializeField]
        private bool useRandomDeviationRange = true;

        [SerializeField, ShowIf(nameof(useRandomDeviationRange), true)]
        private float maxRandomDeviation = 1f;

        public Vector3 GetPosition(int index)
        {
            Vector3 posToReturn = pathPoints[index].transform.position;
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

        public int GetMaxPosition() => pathPoints.Count;

        public void AddNewPathPoint(SPathPoint newPoint) => pathPoints.Add(newPoint);

        public void RemovePathPoint(SPathPoint newPoint) => pathPoints.Remove(newPoint);

        public void Clear()
        {
            foreach (var item in pathPoints)
                DestroyImmediate(item.gameObject);
            pathPoints.Clear();
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            pathPoints.ForEach(item =>
            {
                Vector3 pointPos = item.transform.position;

                Handles.Label(pointPos + Vector3.up, item.gameObject.name);

                Gizmos.DrawSphere(pointPos, 0.1f);
                if (item.leftPoint)
                    Gizmos.DrawLine(pointPos, item.leftPoint.transform.position);
                if (item.rightPoint)
                    Gizmos.DrawLine(pointPos, item.rightPoint.transform.position);
                if (item.upPoint)
                    Gizmos.DrawLine(pointPos, item.upPoint.transform.position);
                if (item.downPoint)
                    Gizmos.DrawLine(pointPos, item.downPoint.transform.position);
            });
        }
#endif
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(SPath))]
    public class SPathEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            SPath patrolPoints = target as SPath;

            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            new Button() { text = "New Point" }
                .OnClick(() =>
                {
                    // ---------------------------------------------------------------------------------------------
                    GameObject spawnedObject = new GameObject(
                        "Point " + (patrolPoints.pathPoints.Count + 1)
                    );

                    SPathPoint pathPoint = spawnedObject.AddComponent<SPathPoint>();
                    pathPoint.sPath = target as SPath;
                    patrolPoints.AddNewPathPoint(pathPoint);
                    spawnedObject.transform.parent = patrolPoints.transform;
                    spawnedObject.transform.localPosition = Vector3.zero;
                    // ---------------------------------------------------------------------------------------------

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
