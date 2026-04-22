namespace SABI
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class SimpleNPCSpawner : MonoBehaviour
    {
        [Header("Spawn Configuration")]
        public GameObject npcPrefab;
        public int spawnCount = 20;
        public float spawnRadius = 30f;
        public Vector3 centerPoint;

        [Header("Height Control")]
        public float maxHeightOffset = 2f;

        [Header("Safety Settings")]
        public float sampleDistance = 5f;

        // New variable added per request
        public float minDistanceBetweenPrefabs = 0.2f;

        [ContextMenu("Spawn")]
        public void SpawnNPCs()
        {
            GameObject rootContainer = GameObject.Find("/NPC");
            if (rootContainer == null)
            {
                rootContainer = new GameObject("NPC");
#if UNITY_EDITOR
                Undo.RegisterCreatedObjectUndo(rootContainer, "Create NPC Container");
#endif
            }

            int successfulSpawns = 0;
            // Keep track of spawned positions to enforce min distance
            List<Vector3> spawnedPositions = new List<Vector3>();

            // We use a slightly higher loop limit or a while loop if you want to
            // guarantee the count, but for simplicity, we'll stick to your loop.
            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                float randomHeight = Random.Range(-maxHeightOffset, maxHeightOffset);

                Vector3 randomPos = new Vector3(
                    centerPoint.x + randomCircle.x,
                    centerPoint.y + randomHeight,
                    centerPoint.z + randomCircle.y
                );

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPos, out hit, sampleDistance, NavMesh.AllAreas))
                {
                    // Check if the position is too close to any previously spawned NPC
                    if (IsPositionValid(hit.position, spawnedPositions))
                    {
                        GameObject npc;

#if UNITY_EDITOR
                        npc = (GameObject)PrefabUtility.InstantiatePrefab(npcPrefab);
                        npc.transform.position = hit.position;
                        npc.transform.rotation = Quaternion.identity;
                        Undo.RegisterCreatedObjectUndo(npc, "Spawn NPC");
#else
                        npc = Instantiate(npcPrefab, hit.position, Quaternion.identity);
#endif
                        npc.transform.SetParent(rootContainer.transform);

                        spawnedPositions.Add(hit.position);
                        successfulSpawns++;
                    }
                }
            }
            Debug.Log(
                $"Spawned {successfulSpawns} NPCs. {(spawnCount - successfulSpawns)} failed due to distance or NavMesh constraints."
            );
        }

        private bool IsPositionValid(Vector3 candidatePos, List<Vector3> existingPositions)
        {
            foreach (Vector3 pos in existingPositions)
            {
                // Use sqrMagnitude for better performance (avoids square root calculation)
                if (
                    Vector3.SqrMagnitude(candidatePos - pos)
                    < (minDistanceBetweenPrefabs * minDistanceBetweenPrefabs)
                )
                {
                    return false;
                }
            }
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            DrawWireDisk(centerPoint, spawnRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(
                centerPoint,
                new Vector3(spawnRadius * 2, maxHeightOffset * 2, spawnRadius * 2)
            );
        }

        private void DrawWireDisk(Vector3 center, float radius)
        {
            float angle = 0;
            Vector3 lastPoint = center + new Vector3(radius, 0, 0);
            for (int i = 1; i <= 32; i++)
            {
                angle += (Mathf.PI * 2) / 32;
                Vector3 nextPoint =
                    center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }
        }
    }
}
