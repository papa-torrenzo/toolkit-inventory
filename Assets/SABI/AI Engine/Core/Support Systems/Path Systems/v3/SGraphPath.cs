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

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(SGraph))]
    public class SGraphPathEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            SGraph patrolPoints = target as SGraph;
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
    public class SGraphPath : MonoBehaviour
    {
        public static List<SGraphPath> Instances = new();

        // public static List<SGraphPath> allPathPoints = new();
        public SGraph sGraph;
        public SGraphPoint point1,
            point2;

        void OnEnable() => SGraphPath.Instances.Add(this);

        void OnDisable() => SGraphPath.Instances.Remove(this);

        public void Init(SGraph sGraph, SGraphPoint point1, SGraphPoint point2)
        {
            this.sGraph = sGraph;
            this.point1 = point1;
            this.point2 = point2;
        }

        public void Validate() { }

        public override bool Equals(object other)
        {
            if (other is SGraphPath)
            {
                SGraphPath path = other as SGraphPath;

                if (path.point1 != point1 && path.point1 != point2)
                    return false;
                if (path.point2 != point1 && path.point2 != point2)
                    return false;

                return true;
            }
            return false;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Vector3 p1Pos = point1.transform.position;
            Vector3 p2Pos = point2.transform.position;

            Handles.Label((p2Pos - p1Pos) * 0.5f + p1Pos, gameObject.name);

            Gizmos.DrawLine(p1Pos, p2Pos);
        }
#endif

        public void DeleteGraphPath()
        {
            sGraph.RemoveGraphPath(this);
            point1.connectedPaths.Remove(this);
            point2.connectedPaths.Remove(this);
            DestroyImmediate(gameObject);
        }
    }
}
