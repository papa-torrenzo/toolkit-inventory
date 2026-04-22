namespace SABI
{
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using Unity.VisualScripting;
#endif
    public class PLOT_ChanceToSpawnPrefabAsChild : PLOT
    {
        [SerializeField, Range(0, 1)]
        protected float chance = 0.5f;

        [SerializeField]
        protected RandomWeightedGameObject prefabs;

        public override void Execute()
        {
            if (!SUtilities.Chance(chance * 100))
                return;

            transform.DestroyChildrenImmediately();
            GameObject spawnedItem;
#if UNITY_EDITOR
            spawnedItem =
                PrefabUtility.InstantiatePrefab(prefabs.GetRandomItem(), transform) as GameObject;
            #else
            spawnedItem 
             = Instantiate(prefabs.GetRandomItem(), transform);
            #endif
            
            spawnedItem.transform.localPosition = Vector3.zero;
            spawnedItem.transform.localRotation = Quaternion.identity;
            if (spawnedItem.TryGetComponent(out PLOT_HandleChildPLOT plot))
            {
                plot.Execute();
            }
        }

        public void SpawnAll()
        {
            foreach (var item in prefabs.ElementsData)
            {
                Object spawnedItem;
#if UNITY_EDITOR
                spawnedItem =
               PrefabUtility.InstantiatePrefab(item.child, transform);
                 #else
                 spawnedItem = Instantiate(item.child, transform);
                 #endif
                (spawnedItem as GameObject).transform.localPosition = Vector3.zero;
                (spawnedItem as GameObject).transform.localRotation = Quaternion.identity;
            }
        }

        public void SetMaxWeightToAll()
        {
            foreach (var item in prefabs.ElementsData)
            {
                item.weight = 1;
            }
        }
    }
    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT_ChanceToSpawnPrefabAsChild))]
    public class PLOT_ChanceToSpawnPrefabAsChildEditor : PLOTEditor
    {
        public override void CustomThings()
        {
            PLOT_ChanceToSpawnPrefabAsChild pLOT_ChanceToSpawnPrefabAsChild =
                target as PLOT_ChanceToSpawnPrefabAsChild;

            base.CustomThings();
            buttons.Add("Spawn All", () => pLOT_ChanceToSpawnPrefabAsChild.SpawnAll());
            buttons.Add(
                "Set Max Weight To All",
                () => pLOT_ChanceToSpawnPrefabAsChild.SetMaxWeightToAll()
            );
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}





// ---------------------------------------------------------------------------------------------
