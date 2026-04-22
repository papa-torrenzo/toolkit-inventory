namespace SABI
{
    using SABI;
    using UnityEngine;
    using UnityEngine.UIElements;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
#endif

    public class PLOT_EnableOneChild : PLOT
    {
        [System.Serializable]
        public class ChildData
        {
            public GameObject child;

            [Range(0, 1)]
            public float weight = 1;

            public Vector2 range = Vector2.zero;
        }

        public ChildData[] childDatas;

        [Button]
        public void Refresh()
        {
            int totalChildrens = transform.childCount;
            childDatas = new ChildData[totalChildrens];
            for (int i = 0; i < totalChildrens; i++)
            {
                childDatas[i] = new ChildData()
                {
                    child = transform.GetChild(i).gameObject,
                    weight = 1,
                };
            }
        }

        public void CalculateRange()
        {
            int childCount = transform.childCount;
            float lastRange = 0;
            for (int i = 0; i < childCount; i++)
            {
                childDatas[i].range = new Vector2(lastRange, lastRange + childDatas[i].weight);
                lastRange += childDatas[i].weight;
            }
        }

        public override void Execute()
        {
            int childCount = transform.childCount;
            float lastRange = 0;
            for (int i = 0; i < childCount; i++)
            {
                childDatas[i].range = new Vector2(lastRange, lastRange + childDatas[i].weight);
                lastRange += childDatas[i].weight;
            }
            float randomNumber = Random.Range(0, lastRange);
            for (int i = 0; i < childCount; i++)
            {
                Vector2 range = childDatas[i].range;
                if (randomNumber >= range.x && randomNumber < range.y)
                    childDatas[i].child.Enable();
                else
                    childDatas[i].child.Disable();
            }
        }
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(PLOT_EnableOneChild))]
    public class PLOT_EnableOneChildEditor : PLOTEditor
    {
        public override void CustomThings()
        {
            base.CustomThings();
            buttons.Add("CalculateRange", () => (target as PLOT_EnableOneChild).CalculateRange());
            buttons.Add("Refresh", () => (target as PLOT_EnableOneChild).Refresh());
        }
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
