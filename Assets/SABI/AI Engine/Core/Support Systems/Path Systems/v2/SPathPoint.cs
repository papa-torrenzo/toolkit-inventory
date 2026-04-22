namespace SABI
{
    using System;
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
    [CustomEditor(typeof(SPathPoint))]
    public class SPathPointEditor : Editor
    {
        SPathPoint targetPathPoint;

        private SPathPoint CreateSPathPoint()
        {
            // ---------------------------------------------------------------------------------------------
            GameObject spawnedObject = new GameObject(
                "Point " + (targetPathPoint.sPath.pathPoints.Count + 1)
            );
            SPathPoint pathPoint = spawnedObject.AddComponent<SPathPoint>();
            pathPoint.sPath = targetPathPoint.sPath;
            // ---------------------------------------------------------------------------------------------
            spawnedObject.transform.localPosition = targetPathPoint.transform.position;
            spawnedObject.transform.parent = targetPathPoint.sPath.transform;
            // ---------------------------------------------------------------------------------------------

            Selection.activeGameObject = spawnedObject;

            targetPathPoint.sPath.AddNewPathPoint(pathPoint);

            // ---------------------------------------------------------------------------------------------
            return pathPoint;
        }

        private SPathPoint GetNearestSPathPoint()
        {
            Vector3 pos = targetPathPoint.transform.position;
            SPathPoint nearestSPathPoint = null;
            float nearestPointSqrDistance = 9999;
            targetPathPoint.sPath.pathPoints.ForEach(item =>
            {
                if (item != targetPathPoint)
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

        public Div ButtonSection(
            string title,
            VisualElement parent,
            Action onUpBtnClick,
            Action onLeftBtnClick,
            Action onRIghtBtnClick,
            Action onDownBtnClick,
            Action commonLogic = null
        )
        {
            return new Div(
                new SABI.Flow.FColumn(
                    new Div().Height(5),
                    new FText(title).SetParent(parent),
                    new Div().Height(5),
                    new FRow(
                        new Button() { text = "UP" }
                            .FixedWidth(60)
                            .OnClick(() =>
                            {
                                onUpBtnClick?.Invoke();
                                commonLogic?.Invoke();
                            })
                    ),
                    new FRow(
                        new Button() { text = "LEFT" }
                            .FixedWidth(60)
                            .OnClick(() =>
                            {
                                onLeftBtnClick?.Invoke();
                                commonLogic?.Invoke();
                            }),
                        new Button() { text = "RIGHT" }
                            .FixedWidth(60)
                            .OnClick(() =>
                            {
                                onRIghtBtnClick?.Invoke();
                                commonLogic?.Invoke();
                            })
                    ),
                    new FRow(
                        new Button() { text = "Down" }
                            .FixedWidth(60)
                            .OnClick(() =>
                            {
                                onDownBtnClick?.Invoke();
                                commonLogic?.Invoke();
                            })
                    ),
                    new Div().Height(5)
                ).CenterF()
            ).SetParent(parent).Border(5, 1, Color.white).BGColor(Color.black.WithA(0.2f)).MarginTop(10);
        }

        public override VisualElement CreateInspectorGUI()
        {
            targetPathPoint = target as SPathPoint;

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

            ButtonSection(
                "Add New",
                root,
                onUpBtnClick: () =>
                {
                    SPathPoint pathPoint = CreateSPathPoint();
                    targetPathPoint.upPoint = pathPoint;
                    pathPoint.downPoint = targetPathPoint;
                },
                onLeftBtnClick: () =>
                {
                    SPathPoint pathPoint = CreateSPathPoint();
                    targetPathPoint.leftPoint = pathPoint;
                    pathPoint.rightPoint = targetPathPoint;
                },
                onRIghtBtnClick: () =>
                {
                    SPathPoint pathPoint = CreateSPathPoint();
                    targetPathPoint.rightPoint = pathPoint;
                    pathPoint.leftPoint = targetPathPoint;
                },
                onDownBtnClick: () =>
                {
                    SPathPoint pathPoint = CreateSPathPoint();
                    targetPathPoint.downPoint = pathPoint;
                    pathPoint.upPoint = targetPathPoint;
                }
            );

            ButtonSection(
                "Connect To Nearest",
                root,
                onUpBtnClick: () =>
                {
                    SPathPoint pathPoint = GetNearestSPathPoint();
                    targetPathPoint.upPoint = pathPoint;
                    pathPoint.downPoint = targetPathPoint;
                },
                onLeftBtnClick: () =>
                {
                    SPathPoint pathPoint = GetNearestSPathPoint();
                    targetPathPoint.leftPoint = pathPoint;
                    pathPoint.rightPoint = targetPathPoint;
                },
                onRIghtBtnClick: () =>
                {
                    SPathPoint pathPoint = GetNearestSPathPoint();
                    targetPathPoint.rightPoint = pathPoint;
                    pathPoint.leftPoint = targetPathPoint;
                },
                onDownBtnClick: () =>
                {
                    SPathPoint pathPoint = GetNearestSPathPoint();
                    targetPathPoint.downPoint = pathPoint;
                    pathPoint.upPoint = targetPathPoint;
                }
            );

            ButtonSection(
                "Merge To Nearest",
                root,
                onUpBtnClick: () =>
                {
                    SPathPoint pathPoint = GetNearestSPathPoint();
                    SPathPoint targetUp = pathPoint.upPoint;
                    if (targetUp != null)
                    {
                        Debug.LogError("Target Up point is alredy set");
                        return;
                    }

                    if (targetPathPoint.upPoint != null && pathPoint.upPoint == null)
                        pathPoint.upPoint = targetPathPoint.upPoint;
                    if (targetPathPoint.leftPoint != null && pathPoint.leftPoint == null)
                        pathPoint.leftPoint = targetPathPoint.leftPoint;
                    if (targetPathPoint.rightPoint != null && pathPoint.rightPoint == null)
                        pathPoint.rightPoint = targetPathPoint.rightPoint;
                    if (targetPathPoint.downPoint != null && pathPoint.downPoint == null)
                        pathPoint.downPoint = targetPathPoint.downPoint;

                    targetPathPoint.sPath.RemovePathPoint(targetPathPoint);
                    DestroyImmediate(targetPathPoint.gameObject);
                },
                onLeftBtnClick: () =>
                {
                    SPathPoint pathPoint = GetNearestSPathPoint();
                    SPathPoint targetLeft = pathPoint.leftPoint;
                    if (targetLeft != null)
                    {
                        Debug.LogError("Target Left point is alredy set");
                        return;
                    }

                    if (targetPathPoint.upPoint != null && pathPoint.upPoint == null)
                        pathPoint.upPoint = targetPathPoint.upPoint;
                    if (targetPathPoint.leftPoint != null && pathPoint.leftPoint == null)
                        pathPoint.leftPoint = targetPathPoint.leftPoint;
                    if (targetPathPoint.rightPoint != null && pathPoint.rightPoint == null)
                        pathPoint.rightPoint = targetPathPoint.rightPoint;
                    if (targetPathPoint.downPoint != null && pathPoint.downPoint == null)
                        pathPoint.downPoint = targetPathPoint.downPoint;

                    targetPathPoint.sPath.RemovePathPoint(targetPathPoint);
                    DestroyImmediate(targetPathPoint.gameObject);
                },
                onRIghtBtnClick: () =>
                {
                    SPathPoint pathPoint = GetNearestSPathPoint();
                    SPathPoint targetRight = pathPoint.rightPoint;
                    if (targetRight != null)
                    {
                        Debug.LogError("Target Right point is alredy set");
                        return;
                    }

                    if (targetPathPoint.upPoint != null && pathPoint.upPoint == null)
                        pathPoint.upPoint = targetPathPoint.upPoint;
                    if (targetPathPoint.leftPoint != null && pathPoint.leftPoint == null)
                        pathPoint.leftPoint = targetPathPoint.leftPoint;
                    if (targetPathPoint.rightPoint != null && pathPoint.rightPoint == null)
                        pathPoint.rightPoint = targetPathPoint.rightPoint;
                    if (targetPathPoint.downPoint != null && pathPoint.downPoint == null)
                        pathPoint.downPoint = targetPathPoint.downPoint;

                    targetPathPoint.sPath.RemovePathPoint(targetPathPoint);
                    DestroyImmediate(targetPathPoint.gameObject);
                },
                onDownBtnClick: () =>
                {
                    SPathPoint pathPoint = GetNearestSPathPoint();
                    SPathPoint targetDown = pathPoint.downPoint;
                    if (targetDown != null)
                    {
                        Debug.LogError("Target Down point is alredy set");
                        return;
                    }

                    if (targetPathPoint.upPoint != null && pathPoint.upPoint == null)
                        pathPoint.upPoint = targetPathPoint.upPoint;
                    if (targetPathPoint.leftPoint != null && pathPoint.leftPoint == null)
                        pathPoint.leftPoint = targetPathPoint.leftPoint;
                    if (targetPathPoint.rightPoint != null && pathPoint.rightPoint == null)
                        pathPoint.rightPoint = targetPathPoint.rightPoint;
                    if (targetPathPoint.downPoint != null && pathPoint.downPoint == null)
                        pathPoint.downPoint = targetPathPoint.downPoint;

                    targetPathPoint.sPath.RemovePathPoint(targetPathPoint);
                    DestroyImmediate(targetPathPoint.gameObject);
                }
            );

            return root;
        }
    }
#endif
    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>

    public class SPathPoint : MonoBehaviour
    {
        public SPath sPath;

        [Header("Connected Path Points")] // -------------------------------
        public SPathPoint leftPoint;
        public SPathPoint rightPoint,
            upPoint,
            downPoint;

        public void Init(SPath sPath)
        {
            this.sPath = this.sPath;
            this.sPath.AddNewPathPoint(this);
        }

        void OnDestroy()
        {
            sPath.RemovePathPoint(this);
        }
    }
}
