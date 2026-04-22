using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SABI
{
    public class Sensor_LOS : Sensor_Base
    {
        [Header("Sensor Settings")]
        public float detectionRange = 10f;
        public float coneAngle = 45f;
        public LayerMask layersToCheck;

        public event Action<GameObject, string, bool> OnTargetDetectionChange;

        List<GameObject> targetsVisible = new(),
            previewsFramTargetsVisible = new();

        void Update()
        {
            ScanForTargets();
        }

        private void ScanForTargets()
        {
            previewsFramTargetsVisible.Clear();
            targetsVisible.ForEach(item => previewsFramTargetsVisible.Add(item));
            targetsVisible.Clear();

            List<Collider> targetsInRange = Physics
                .OverlapSphere(transform.position, detectionRange)
                .ToList();

            foreach (Collider target in targetsInRange)
            {
                if (target.TryGetComponent(out LOSTarget lodTarget))
                {
                    Vector3 directionToTarget = (
                        target.transform.position - transform.position
                    ).normalized;
                    float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                    if (angleToTarget <= coneAngle / 2)
                    {
                        if (IsTargetVisible(target))
                        {
                            targetsVisible.Add(target.gameObject);

                            if (!previewsFramTargetsVisible.Contains(target.gameObject))
                            {
                                OnTargetDetectionChange?.Invoke(
                                    target.gameObject,
                                    lodTarget.GetLosTargetTag(),
                                    true
                                );
                                Debug.Log($"[SAB] LOS Target Found");
                            }
                        }
                    }
                }
            }

            foreach (var item in previewsFramTargetsVisible)
            {
                if (!targetsVisible.Contains(item))
                {
                    OnTargetDetectionChange?.Invoke(
                        item.gameObject,
                        item.gameObject.GetComponent<LOSTarget>().GetLosTargetTag(),
                        false
                    );

                    Debug.Log($"[SAB] LOS Target Lost");
                }
            }
        }

        private bool IsTargetVisible(Collider target)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            RaycastHit hit;

            if (
                Physics.Raycast(
                    transform.position,
                    directionToTarget,
                    out hit,
                    detectionRange,
                    layersToCheck
                )
            )
            {
                return hit.collider == target;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            // Visualize detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Visualize cone of vision
            Gizmos.color = Color.red;
            Vector3 leftBoundary =
                Quaternion.Euler(0, -coneAngle / 2, 0) * transform.forward * detectionRange;
            Vector3 rightBoundary =
                Quaternion.Euler(0, coneAngle / 2, 0) * transform.forward * detectionRange;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        }
    }
}
