namespace SABI
{
    using SABI;
    using UnityEngine;

    public class SpawnNpcGroup : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private float spawnRadius = 2;

        [SerializeField, Range(0, 1)]
        private float spawnRadiusRandomness = 0;

        [SerializeField]
        private float spawnAmount;

        [SerializeField, Range(0, 1)]
        private float spawnAmountRandomness = 0;

        [Button, ContextMenu("Spawn")]
        private void Spawn()
        {
            if (prefab == null)
            {
                Debug.LogError("Spawn(): prefab is null");
                return;
            }

            int finalSpawnRadius = (
                spawnRadius + (spawnRadius * spawnRadiusRandomness)
            ).FloorToInt();
            int finalSpawnAmount = (
                spawnAmount + (spawnAmount * spawnAmountRandomness)
            ).FloorToInt();

            Terrain terrain = Terrain.activeTerrain;

            for (int i = 0; i < finalSpawnAmount; i++)
            {
                Vector2 rand = Random.insideUnitCircle * finalSpawnRadius;
                Vector3 spawnPos = transform.position + new Vector3(rand.x, 0f, rand.y);

                if (terrain != null)
                {
                    spawnPos.y = terrain.SampleHeight(spawnPos) + terrain.transform.position.y;
                }
                else
                {
                    Ray down = new Ray(spawnPos + Vector3.up * 1000f, Vector3.down);
                    if (
                        Physics.Raycast(
                            down.origin,
                            down.direction,
                            out RaycastHit hit,
                            2000f,
                            Physics.DefaultRaycastLayers,
                            QueryTriggerInteraction.Ignore
                        )
                    )
                    {
                        spawnPos = hit.point;
                    }
                }

                Vector3 lookDir = transform.position - spawnPos;
                lookDir.y = 0f;
                Quaternion rot =
                    lookDir.sqrMagnitude > 0.0001f
                        ? Quaternion.LookRotation(lookDir)
                        : Quaternion.identity;

#if UNITY_EDITOR
                GameObject instance =
                    UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (instance != null)
                {
                    instance.transform.position = spawnPos;
                    instance.transform.rotation = rot;
                    instance.transform.SetParent(null);
                }
#else
                GameObject instance = Instantiate(prefab, spawnPos, rot, transform);
#endif
            }
        }
    }
}
