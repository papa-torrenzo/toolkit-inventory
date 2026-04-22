namespace SABI
{

using UnityEngine;

[System.Serializable]
public class RandomWeightedGameObject
{
    public ObjectData[] ElementsData;

    [System.Serializable]
    public class ObjectData
    {
        public GameObject child;

        [Range(0, 1)]
        public float weight = 1;

        [ReadOnly]
        public Vector2 range = Vector2.zero;
    }

    public void CalculateRange()
    {
        int childCount = ElementsData.Length;
        float lastRange = 0;
        for (int i = 0; i < childCount; i++)
        {
            ElementsData[i].range = new Vector2(lastRange, lastRange + ElementsData[i].weight);
            lastRange += ElementsData[i].weight;
        }

        if (lastRange == 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                ElementsData[i].weight = 1;
                ElementsData[i].range = new Vector2(lastRange, lastRange + ElementsData[i].weight);
                lastRange += ElementsData[i].weight;
            }
        }
    }

    public GameObject GetRandomItem()
    {
        if (ElementsData.Length == 0)
            throw new System.Exception("ElementsData.Length = 0");

        // Debug.LogError("ElementsData is empty");

        CalculateRange();

        int childCount = ElementsData.Length;
        float lastRange = 0;
        for (int i = 0; i < childCount; i++)
        {
            ElementsData[i].range = new Vector2(lastRange, lastRange + ElementsData[i].weight);
            lastRange += ElementsData[i].weight;
        }
        float randomNumber = Random.Range(0, lastRange);

        for (int i = 0; i < childCount; i++)
        {
            Vector2 range = ElementsData[i].range;
            if (randomNumber >= range.x && randomNumber < range.y)
                return ElementsData[i].child;
        }

        Debug.Log($"[SAB] Somethings wrong REF:", ElementsData[0].child);
        throw new System.Exception("Somethings wrong");
    }
}

}