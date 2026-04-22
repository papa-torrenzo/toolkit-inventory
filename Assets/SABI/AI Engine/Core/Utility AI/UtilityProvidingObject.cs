namespace SABI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    // using UnityEngine.Splines;

    public class UtilityProvidingObject : MonoBehaviour
    {
        public static List<UtilityProvidingObject> AllIntractableObjects = new();
        public List<IntractableObjectData> intractableObjectDatas;

        public bool isUsable = true;
        public float interactionDuration = 1;
        public float interactionTimeLeft;
        public string animationName = "idle";
        public Vector3 relativeOffsetPosition = new Vector3(0, 0, 1),
            relativeOffsetRotation = new Vector3(0, 180, 0);

        void OnEnable() => UtilityProvidingObject.AllIntractableObjects.Add(this);

        void OnDisable() => UtilityProvidingObject.AllIntractableObjects.Remove(this);

        [SerializeField]
        private bool isOnNavmesh;

        void Start()
        {
            float maxDistance = 0.1f;
            int areaMask = NavMesh.AllAreas;
            isOnNavmesh = NavMesh.SamplePosition(
                GetMovePosition(),
                out NavMeshHit hit,
                maxDistance,
                areaMask
            );
            if (!isOnNavmesh)
            {
                Debug.Log($"[SAB] [A] Not on Navmesh ", this.gameObject);
            }
        }

        public Vector3 GetMovePosition() =>
            transform.position
            + transform.forward * relativeOffsetPosition.z
            + transform.up * relativeOffsetPosition.y
            + transform.right * relativeOffsetPosition.x;

        public Vector3 GetRotation() => transform.eulerAngles + relativeOffsetRotation;

        public void StartInteraction()
        {
            isUsable = false;
            interactionTimeLeft = interactionDuration;
        }

        public void InteractionCompleted(StatusSystem status)
        {
            interactionTimeLeft = interactionDuration;
            isUsable = true;
            status.OnInteract(this);
        }

        // void OnDrawGizmos()
        // {
        //     float progress = isUsable ? 0f : (interactionTimeLeft / interactionDuration);
        //     progress = Mathf.Clamp01(progress);

        //     Vector3 center = transform.position + Vector3.up * 2.2f;
        //     float fullWidth = 2f;

        //     Gizmos.color = Color.gray;
        //     Gizmos.DrawWireCube(center, new Vector3(fullWidth, 0.1f, 0.1f));

        //     Gizmos.color = Color.green;
        //     Vector3 barScale = new Vector3(fullWidth * progress, 0.1f, 0.1f);

        //     Vector3 barPos = center - new Vector3((fullWidth * (1 - progress)) / 2, 0, 0);

        //     Gizmos.DrawCube(barPos, barScale);
        // }

        void OnDrawGizmosSelected()
        {
            // Matrix4x4 rotationMatrix = Matrix4x4.TRS(
            //     transform.position + relativeOffsetPosition,
            //     Quaternion.Euler(transform.eulerAngles + relativeOffsetPosition),
            //     Vector3.one * 1f
            // );

            // Gizmos.matrix = rotationMatrix;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(GetMovePosition(), 0.2f);

            // Gizmos.DrawRay(
            //     transform.position + relativeOffsetPosition,
            //     (
            //         (transform.position + relativeOffsetPosition)
            //         + (transform.rotation.eulerAngles + relativeOffsetRotation)
            //     )
            //         - transform.position
            //         + relativeOffsetPosition
            // );
        }

        internal void Tick(Action OnComplete)
        {
            interactionTimeLeft -= Time.deltaTime;

            if (interactionTimeLeft <= 0)
            {
                OnComplete?.Invoke();
            }
        }
    }

    [System.Serializable]
    public class IntractableObjectData
    {
        public StatusElementType statusElementType;
        public EnumAddRemove addOrRemove;

        [Range(0, 1)]
        public float value = 0.1f;
    }

    public enum EnumAddRemove
    {
        Add,
        Remove,
    }
}
