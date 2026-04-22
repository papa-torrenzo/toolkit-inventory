namespace SABI
{
    using SABI;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif
    public class SpawnGameobjectInPathPoints : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private SLinearPath patrolPoints;

        [SerializeField]
        private int spawnAmount = 3;

        [SerializeField]
        private float randomDeviation = 1;

        [Button("Hello"), ContextMenu("Spawn()")]
        private void Spawn()
        {
            if (patrolPoints == null)
                patrolPoints = GetComponent<SLinearPath>();
            int finalSpawnAmount = (spawnAmount + (spawnAmount * randomDeviation)).FloorToInt();
            for (int i = 0; i < finalSpawnAmount; i++)
            {
#if UNITY_EDITOR
                GameObject instance = null;

                if (prefab == null)
                {
                    Debug.LogWarning("Spawn(): prefab reference is null");
                }
                else
                {
                    // If the reference is a prefab asset, instantiate via PrefabUtility.
                    if (PrefabUtility.IsPartOfPrefabAsset(prefab))
                    {
                        instance =
                            UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    }
                    else
                    {
                        // reference is likely a scene/gameobject instance - duplicate it.
                        instance = Object.Instantiate(prefab);
                        instance.name = prefab.name;
                    }

                    if (instance != null)
                    {
                        instance.transform.position = patrolPoints.GetPosition(
                            Random.Range(0, patrolPoints.GetMaxPosition())
                        );
                        instance.transform.SetParent(null);
                        Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
                        EditorSceneManager.MarkSceneDirty(instance.scene);
                    }
                }
#endif
            }
        }
    }
}
