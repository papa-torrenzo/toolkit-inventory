// using UnityEngine;
// using UnityEngine.Splines;

// namespace SABI
// {
//     public class State_SplineMovement : State_Base
//     {
//         public SplineContainer spline;
//         public float speed = 0.01f;
//         private float progress;

//         [SerializeField]
//         private Vector3 offset;

//         float minDistanceToSpline = 2;

//         public override void StateEnter()
//         {
//             base.StateEnter();
//             Vector3 splinePosition = spline.EvaluatePosition(progress);
//         }

//         public override void StateUpdate()
//         {
//             base.StateUpdate();

//             progress += speed * Time.deltaTime;
//             if (progress >= 1)
//                 progress = 0;

//             Vector3 splinePosition = spline.EvaluatePosition(progress);
//             Vector3 tangent = spline.EvaluateTangent(progress);
//             Quaternion splineRotation = Quaternion.LookRotation(tangent);

//             float distancetoSpline = Vector3.Distance(
//                 baseStateMachine.transform.position,
//                 splinePosition
//             );
//             if (distancetoSpline > minDistanceToSpline)
//             {
//                 if (baseStateMachine is StateMachine)
//                 {
//                     (baseStateMachine as StateMachine).navMeshManager.SetDestination(
//                         splinePosition
//                     );
//                 }
//                 else
//                 {
//                     Debug.LogError("stateMachine is not StateMachine", this);
//                 }
//             }
//             else
//             {
//                 baseStateMachine.transform.position = splinePosition + offset;
//                 baseStateMachine.transform.rotation = splineRotation;
//             }
//         }
//     }
// }
