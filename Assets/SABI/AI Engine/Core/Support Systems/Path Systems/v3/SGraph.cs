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
    using System;
#endif

    public class SGraph : MonoBehaviour
    {
        [SerializeField]
        private bool visualizePath = true;

        public static List<SGraph> Instances = new();

        public List<SGraphPoint> graphPoints = new();
        public List<SGraphPath> graphPaths = new();

        [SerializeField]
        private bool useRandomDeviationRange = true;

        [SerializeField, ShowIf(nameof(useRandomDeviationRange), true)]
        private float maxRandomDeviation = 1f;

        void OnEnable() => Instances.Add(this);

        void OnDisable() => Instances.Remove(this);

        public int GetMaxPosition() => graphPaths.Count;

        public void AddNewGraphPoint(SGraphPoint newPoint) => graphPoints.Add(newPoint);

        public void AddNewGraphPath(SGraphPath newPoint) => graphPaths.Add(newPoint);

        public void RemoveGraphPoint(SGraphPoint newPoint) => graphPoints.Remove(newPoint);

        public void RemoveGraphPath(SGraphPath newPoint) => graphPaths.Remove(newPoint);

        public void Clear()
        {
            foreach (var item in graphPaths)
                DestroyImmediate(item.gameObject);
            graphPaths.Clear();
            graphPoints.Clear();
        }

        public void Validate()
        {
            graphPaths.ForEach(item =>
            {
                graphPaths.ForEach(item2 =>
                {
                    if (item.Equals(item2))
                    {
                        
                    }
                });
            });
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(SGraph))]
    public class SGraphEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            SGraph graph = target as SGraph;

            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            new Button() { text = "New Point" }
                .OnClick(() =>
                {
                    // ---------------------------------------------------------------------------------------------

                    GameObject pointsParentObj = new GameObject("Graph Points");
                    GameObject pathsParentObj = new GameObject("Graph Paths");
                    pointsParentObj.transform.parent = graph.transform;
                    pointsParentObj.transform.localPosition = Vector3.zero;
                    pathsParentObj.transform.parent = graph.transform;
                    pathsParentObj.transform.localPosition = Vector3.zero;

                    GameObject spawnedObject = new GameObject(
                        "Point " + (graph.graphPoints.Count + 1)
                    );

                    SGraphPoint pathPoint = spawnedObject.AddComponent<SGraphPoint>();
                    pathPoint.sGraph = target as SGraph;
                    graph.AddNewGraphPoint(pathPoint);
                    spawnedObject.transform.parent = pointsParentObj.transform;
                    spawnedObject.transform.localPosition = Vector3.zero;
                    // ---------------------------------------------------------------------------------------------

                    // ---------------------------------------------------------------------------------------------
                    Selection.activeGameObject = spawnedObject;
                })
                .SetParent(root);
            new Button(() => graph.Clear()) { text = "Clear" }.SetParent(root);
            return root;
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
