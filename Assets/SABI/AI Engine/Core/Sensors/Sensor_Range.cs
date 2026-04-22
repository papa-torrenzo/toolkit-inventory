using System;
using UnityEngine;

namespace SABI
{
    public class Sensor_Range : Sensor_Base
    {
        public Action<GameObject, string, bool> OnTargetDetectionChange;

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out LOSTarget target))
            {
                Debug.Log($"[SAB] OnTriggerEnter() with tag {target.GetLosTargetTag()}");
                OnTargetDetectionChange?.Invoke(other.gameObject, target.GetLosTargetTag(), true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out LOSTarget target))
            {
                Debug.Log($"[SAB] OnTriggerExit() with tag {target.GetLosTargetTag()}");
                OnTargetDetectionChange?.Invoke(other.gameObject, target.GetLosTargetTag(), false);
            }
        }
    }
}
