namespace SABI
{
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class PLOT_SpawnPrefabAsChildForEachChildren : PLOT_ChanceToSpawnPrefabAsChild
    {
        [SerializeField]
        private RandomWeightedGameObject position;

        [SerializeField]
        private int maxSpawn = 1;

        [SerializeField]
        private bool keepPrefabsToSpawnUnique = false;

        [SerializeField]
        private bool keepChildGOToSpawnToUnique = true;
        List<GameObject> uniquePrefabs,
            uniqueChildGOToSpawnTo;

        public override void Execute()
        {
            if (!SUtilities.Chance(chance * 100))
                return;

            uniquePrefabs = new List<GameObject>();
            uniqueChildGOToSpawnTo = new List<GameObject>();

            int maxSpawnValue = maxSpawn; //.GetValue();

            if (position.ElementsData.Length == 0)
            {
                FillAllChildren();
            }
            // ---------------------------------------------------------------------------------------------
            if (keepChildGOToSpawnToUnique)
            {
                position
                    .ElementsData.GetUniqeRandomItems(maxSpawnValue)
                    .ForEach(item => uniqueChildGOToSpawnTo.Add(item.child));
            }
            else
            {
                for (int i = 0; i < maxSpawnValue; i++)
                {
                    uniqueChildGOToSpawnTo.Add(position.ElementsData.GetRandomItem().child);
                }
            }
            // ---------------------------------------------------------------------------------------------
            if (keepPrefabsToSpawnUnique)
            {
                prefabs
                    .ElementsData.GetUniqeRandomItems(maxSpawnValue)
                    .ForEach(item => uniquePrefabs.Add(item.child));
            }
            else
            {
                for (int i = 0; i < maxSpawnValue; i++)
                {
                    uniquePrefabs.Add(prefabs.ElementsData.GetRandomItem().child);
                }
            }
            // ---------------------------------------------------------------------------------------------

            transform.ForEachChildren(child => child.DestroyChildrenImmediately());

            for (int i = 0; i < maxSpawnValue; i++)
            {
                Object spawnedItem;
#if UNITY_EDITOR
                spawnedItem = PrefabUtility.InstantiatePrefab(
                    uniquePrefabs[i],
                    uniqueChildGOToSpawnTo[i].transform
                );
#else
                spawnedItem = Instantiate(uniquePrefabs[i], uniqueChildGOToSpawnTo[i].transform);
#endif

                (spawnedItem as GameObject).transform.localRotation = Quaternion.identity;
                (spawnedItem as GameObject).transform.localPosition = Vector3.zero;
            }
        }

        public void FillAllChildren()
        {
            int childCount = transform.childCount;
            position.ElementsData = new RandomWeightedGameObject.ObjectData[childCount];
            transform.ForEachChildren(
                (childTrans, index) =>
                    position.ElementsData[index] = new RandomWeightedGameObject.ObjectData()
                    {
                        child = childTrans.gameObject,
                        weight = 1,
                    }
            );
            position.CalculateRange();
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>
#if UNITY_EDITOR
    [CustomEditor(typeof(PLOT_SpawnPrefabAsChildForEachChildren))]
    public class PLOT_SpawnPrefabAsChildForEachChildrenEditor
        : PLOT_ChanceToSpawnPrefabAsChildEditor
    {
        public override void CustomThings()
        {
            base.CustomThings();
            buttons.Add(
                "Fill All Children",
                () =>
                {
                    (target as PLOT_SpawnPrefabAsChildForEachChildren).FillAllChildren();
                }
            );
        }
    }
#endif
    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
