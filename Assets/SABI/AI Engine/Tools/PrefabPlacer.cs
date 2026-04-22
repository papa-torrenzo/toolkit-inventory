namespace SABI
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;

    [System.Serializable]
    public struct MinSpawnDistance
    {
        public float minDistance;

        [Range(0f, 1f)]
        public float randomness;
    }

    [CustomPropertyDrawer(typeof(MinSpawnDistance))]
    public class MinSpawnDistanceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var minProp = property.FindPropertyRelative(nameof(MinSpawnDistance.minDistance));
            var randProp = property.FindPropertyRelative(nameof(MinSpawnDistance.randomness));

            // layout: label on the left, two fields on the right
            var labelRect = new Rect(
                position.x,
                position.y,
                EditorGUIUtility.labelWidth,
                position.height
            );
            var fieldsRect = new Rect(
                position.x + EditorGUIUtility.labelWidth,
                position.y,
                position.width - EditorGUIUtility.labelWidth,
                position.height
            );

            EditorGUI.LabelField(labelRect, label);

            float half = fieldsRect.width * 0.5f;
            var r1 = new Rect(fieldsRect.x, fieldsRect.y, half - 4, fieldsRect.height);
            var r2 = new Rect(fieldsRect.x + half + 4, fieldsRect.y, half - 4, fieldsRect.height);

            EditorGUI.PropertyField(r1, minProp, new GUIContent("Min"));
            EditorGUI.PropertyField(r2, randProp, new GUIContent("Rand"));

            // sanitize
            minProp.floatValue = Mathf.Max(0f, minProp.floatValue);
            randProp.floatValue = Mathf.Clamp01(randProp.floatValue);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    public class PrefabPlacer : EditorWindow
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private float spawnRadius = 2f;

        [SerializeField, Range(0f, 1f)]
        private float spawnRadiusRandomness = 0f;

        [SerializeField]
        private int spawnAmount = 1;

        [SerializeField, Range(0f, 1f)]
        private float spawnAmountRandomness = 0f;

        [SerializeField]
        private bool enableSpawning = true;

        [SerializeField]
        private MinSpawnDistance minSpawnDistance;

        // SerializedObject + property refs for drawing
        private SerializedObject so;
        private SerializedProperty prefabProp;
        private SerializedProperty spawnRadiusProp;
        private SerializedProperty spawnRadiusRandomnessProp;
        private SerializedProperty spawnAmountProp;
        private SerializedProperty spawnAmountRandomnessProp;
        private SerializedProperty enableSpawningProp;
        private SerializedProperty minSpawnDistanceProp;

        [MenuItem("Tools/Sabi/PrefabPlacer")]
        private static void ShowWindow()
        {
            var window = GetWindow<PrefabPlacer>();
            window.titleContent = new GUIContent("PrefabPlacer");
            window.Show();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EnsureSerializedObject();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void EnsureSerializedObject()
        {
            if (so == null || prefabProp == null)
            {
                so = new SerializedObject(this);
                prefabProp = so.FindProperty(nameof(prefab));
                spawnRadiusProp = so.FindProperty(nameof(spawnRadius));
                spawnRadiusRandomnessProp = so.FindProperty(nameof(spawnRadiusRandomness));
                spawnAmountProp = so.FindProperty(nameof(spawnAmount));
                spawnAmountRandomnessProp = so.FindProperty(nameof(spawnAmountRandomness));
                enableSpawningProp = so.FindProperty(nameof(enableSpawning));
                minSpawnDistanceProp = so.FindProperty(nameof(minSpawnDistance));
            }
        }

        void OnGUI()
        {
            EnsureSerializedObject();
            so.Update();

            EditorGUILayout.PropertyField(prefabProp);
            EditorGUILayout.PropertyField(enableSpawningProp);
            EditorGUILayout.PropertyField(spawnRadiusProp);
            EditorGUILayout.PropertyField(spawnRadiusRandomnessProp);
            EditorGUILayout.PropertyField(spawnAmountProp);
            EditorGUILayout.PropertyField(spawnAmountRandomnessProp);
            EditorGUILayout.PropertyField(minSpawnDistanceProp);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Click in Scene view (left click) to spawn prefabs (if enabled). Hold Alt to orbit without spawning.",
                MessageType.Info
            );

            // if (GUILayout.Button("Spawn at Scene View Pivot"))
            // {
            //     if (enableSpawning)
            //         Spawn(SceneView.lastActiveSceneView.pivot);
            //     else
            //         Debug.Log("Spawning is disabled.");
            // }

            so.ApplyModifiedProperties();
        }

        void CreateGUI() { }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!enableSpawning)
                return;

            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Vector3 spawnPos;
                if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
                {
                    spawnPos = hit.point;
                }
                else
                {
                    Plane ground = new Plane(Vector3.up, Vector3.zero);
                    if (!ground.Raycast(ray, out float enter))
                        return;
                    spawnPos = ray.GetPoint(enter);
                }

                Spawn(spawnPos);

                e.Use();
            }
        }

        private void Spawn(Vector3 center)
        {
            if (prefab == null)
            {
                Debug.LogError("Spawn(): prefab is null");
                return;
            }

            // sanitize inputs
            spawnRadius = Mathf.Max(0f, spawnRadius);
            spawnAmount = Mathf.Max(0, spawnAmount);
            minSpawnDistance.minDistance = Mathf.Max(0f, minSpawnDistance.minDistance);
            minSpawnDistance.randomness = Mathf.Clamp01(minSpawnDistance.randomness);

            // compute final amount with randomness
            int extra = Mathf.FloorToInt(spawnAmount * spawnAmountRandomness);
            int finalSpawnAmount = spawnAmount + Random.Range(0, extra + 1);

            Terrain terrain = Terrain.activeTerrain;

            // start undo group so all creations are undone together
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Spawn Prefabs");

            var created = new List<GameObject>(finalSpawnAmount);

            int maxAttemptsPerInstance = 20;

            for (int i = 0; i < finalSpawnAmount; i++)
            {
                GameObject instance = null;
                bool placed = false;

                for (int attempt = 0; attempt < maxAttemptsPerInstance; attempt++)
                {
                    // vary radius per-instance if requested
                    float radiusFactor = Random.Range(
                        1f - spawnRadiusRandomness,
                        1f + spawnRadiusRandomness
                    );
                    float usedRadius = spawnRadius * radiusFactor;
                    Vector2 rand = Random.insideUnitCircle * usedRadius;
                    Vector3 spawnPos = center + new Vector3(rand.x, 0f, rand.y);

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

                    // compute rotation facing center
                    Vector3 lookDir = center - spawnPos;
                    lookDir.y = 0f;
                    Quaternion rot =
                        lookDir.sqrMagnitude > 0.0001f
                            ? Quaternion.LookRotation(lookDir)
                            : Quaternion.identity;

                    // compute min distance for this try (with randomness)
                    float minBase = minSpawnDistance.minDistance;
                    float minFactor = Random.Range(
                        1f - minSpawnDistance.randomness,
                        1f + minSpawnDistance.randomness
                    );
                    float usedMin = minBase * minFactor;

#if UNITY_EDITOR
                    // check distance against already created objects and existing scene objects
                    bool tooClose = false;

                    foreach (var g in created)
                    {
                        if (
                            Vector3.SqrMagnitude(g.transform.position - spawnPos)
                            < usedMin * usedMin
                        )
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        // check against other scene objects (active)
                        var allObjs = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                        foreach (var go in allObjs)
                        {
                            if (created.Contains(go))
                                continue;
                            if (go.transform == null)
                                continue;
                            // ignore objects in preview or disabled?
                            if (!go.activeInHierarchy)
                                continue;
                            if (
                                Vector3.SqrMagnitude(go.transform.position - spawnPos)
                                < usedMin * usedMin
                            )
                            {
                                tooClose = true;
                                break;
                            }
                        }
                    }

                    if (tooClose)
                    {
                        // attempt another random location
                        continue;
                    }

                    instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    if (instance != null)
                    {
                        instance.transform.position = spawnPos;
                        instance.transform.rotation = rot;
                        instance.transform.SetParent(null);

                        // register creation with Undo
                        Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
                        created.Add(instance);

                        // mark scene dirty so changes persist
                        EditorSceneManager.MarkSceneDirty(instance.scene);
                        placed = true;
                        break;
                    }
#else
                instance = Instantiate(prefab, spawnPos, rot, transform);
                placed = instance != null;
                if (placed) created.Add(instance);
                break;
#endif
                } // attempts

                if (!placed)
                {
                    Debug.Log(
                        $"PrefabPlacer: could not place instance {i + 1} after {maxAttemptsPerInstance} attempts (min distance constraint)."
                    );
                }
            }

            // collapse undo operations into the group we started
            Undo.CollapseUndoOperations(undoGroup);

#if UNITY_EDITOR
            // select created objects for convenience
            if (created.Count > 0)
                Selection.objects = created.ToArray();
#endif
        }
    }
#endif
}
