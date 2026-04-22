namespace SABI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SABI;
    using SABI.Flow;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>
#if UNITY_EDITOR
    [CustomEditor(typeof(SGraphPoint))]
    public class SGraphPointEditor : Editor
    {
        SGraphPoint graphPoint;

        public override VisualElement CreateInspectorGUI()
        {
            graphPoint = target as SGraphPoint;

            var root = new VisualElement();
            VisualElement defaultElements = new();
            defaultElements
                .SetParent(root)
                .Border()
                .BorderRadius(5)
                .BorderWidth(1)
                .BGColor(Color.black.WithA(0.2f))
                .Padding(10)
                .MarginTop(3);
            InspectorElement.FillDefaultInspector(defaultElements, serializedObject, this);

            new FHorizontalButtonGroup(
                new Dictionary<string, Action>()
                {
                    { "Add", () => graphPoint.CreateGraphPoint() },
                    { "Delete", graphPoint.DeleteGraphPoint },
                    { "Connect", graphPoint.ConnectGraphPoint },
                    { "Merge", graphPoint.MergeGraphPoint },
                }
            ).SetParent(root).MarginTop(10);

            return root;
        }
    }
#endif
    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>

    public class SGraphPoint : MonoBehaviour
    {
        public static List<SGraphPoint> Instances = new();
        public SGraph sGraph;

        [Header("Connected Paths  Points")] // -------------------------------
        public List<SGraphPoint> connectedPoints = new();
        public List<SGraphPath> connectedPaths = new();

        void OnEnable() => SGraphPoint.Instances.Add(this);

        void OnDisable() => SGraphPoint.Instances.Remove(this);

        public void AddPoint(SGraphPoint point)
        {
            connectedPoints.Add(point);
            // sGraph.AddNewGraphPoint(point);
        }

        public void AddPath(SGraphPath path)
        {
            connectedPaths.Add(path);
            // sGraph.AddNewGraphPath(path);
        }

        public void RemovePoint(SGraphPoint point)
        {
            connectedPoints.Remove(point);
            // sGraph.RemoveGraphPoint(point);
        }

        public void RemovePath(SGraphPath path)
        {
            connectedPaths.Remove(path);
            // sGraph.RemoveGraphPath(path);
        }

        public void Init(SGraph sPath)
        {
            this.sGraph = sPath;
        }

        void OnDestroy() => sGraph.RemoveGraphPoint(this);

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Vector3 pointPos = transform.position;

            Handles.Label(pointPos + Vector3.up, gameObject.name);

            Gizmos.DrawSphere(pointPos, 0.1f);
        }
#endif

        public SGraphPoint CreateGraphPoint()
        {
            // ---------------------------------------------------------------------------------------------
            GameObject spawnedPoint = new GameObject("Point " + (sGraph.graphPoints.Count + 1));
            GameObject spawnedPath = new GameObject(
                "* Path "
                    + (sGraph.graphPaths.Count + 1)
                    + " [ "
                    + gameObject.name.Replace("Point ", "")
                    + " -> "
                    + spawnedPoint.name.Replace("Point ", "")
                    + " ]"
            );

            SGraphPoint newPoint = spawnedPoint.AddComponent<SGraphPoint>();
            newPoint.Init(sGraph);

            SGraphPath newPath = spawnedPath.AddComponent<SGraphPath>();
            newPath.Init(sGraph, this, newPoint);

            // ---------------------------------------------------------------------------------------------
            spawnedPoint.transform.parent = sGraph.transform.Find("Graph Points");
            spawnedPath.transform.parent = sGraph.transform.Find("Graph Paths");
            spawnedPoint.transform.position = transform.position;
            spawnedPath.transform.position = Vector3.zero;
            // ---------------------------------------------------------------------------------------------

#if UNITY_EDITOR
            Selection.activeGameObject = spawnedPoint;
#endif

            AddPath(newPath);
            AddPoint(newPoint);
            newPoint.AddPath(newPath);
            newPoint.AddPoint(this);

            sGraph.AddNewGraphPath(newPath);
            sGraph.AddNewGraphPoint(newPoint);

            // ---------------------------------------------------------------------------------------------
            return newPoint;
        }

        public SGraphPoint GetNearestSPathPoint()
        {
            Vector3 pos = transform.position;
            SGraphPoint nearestSPathPoint = null;
            float nearestPointSqrDistance = 9999;
            sGraph.graphPoints.ForEach(item =>
            {
                if (item != this)
                {
                    float dis = pos.SqrDistance(item.transform.position);
                    if (dis < nearestPointSqrDistance)
                    {
                        nearestPointSqrDistance = dis;
                        nearestSPathPoint = item;
                    }
                }
            });

            return nearestSPathPoint;
        }

        public void MergeGraphPoint()
        {
            SGraphPoint nearestPoint = GetNearestSPathPoint();

            connectedPaths.ForEach(item =>
            {
                if (item.point1 == this && item.point2 != nearestPoint)
                {
                    item.point1 = nearestPoint;
                }

                if (item.point2 == this && item.point1 != nearestPoint)
                {
                    item.point2 = nearestPoint;
                }
            });
            connectedPoints.ForEach(item =>
            {
                bool nearestPointAvailable = item.connectedPoints.Contains(nearestPoint);

                if (!nearestPointAvailable)
                    item.connectedPoints.Replace(this, nearestPoint);
                else
                    item.connectedPoints.Remove(this);
            });

            sGraph.Validate();

            sGraph.RemoveGraphPoint(this);
            DestroyImmediate(gameObject);
        }

        public void DeleteGraphPoint()
        {
            connectedPaths
                .ToList()
                .ForEach(item =>
                {
                    item.DeleteGraphPath();
                });

            connectedPoints.ForEach(otherPoint => otherPoint.connectedPoints.Remove(this));

            sGraph.RemoveGraphPoint(this);
            DestroyImmediate(gameObject);
        }

        public void ConnectGraphPoint()
        {
            SGraphPoint nearestPoint = GetNearestSPathPoint();

            GameObject spawnedPath = new GameObject(
                "* Path "
                    + (sGraph.graphPaths.Count + 1)
                    + " [ "
                    + gameObject.name.Replace("Point ", "")
                    + " -> "
                    + nearestPoint.gameObject.name.Replace("Point ", "")
                    + " ]"
            );

            spawnedPath.transform.parent = sGraph.transform.Find("Graph Paths");

            spawnedPath.AddComponent<SGraphPath>().Init(sGraph, this, nearestPoint);
        }
    }
}
