using UnityEngine;

[System.Serializable]
public class DropTableData
{
    public GameObject prefab;
    public int dropChance;
}

[System.Serializable]
public class DropTable
{
    [SerializeField] private DropTableData[] data;

    public void Drop(Vector2 position)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (Utilities.Roll(data[i].dropChance))
            {
                Object.Instantiate(data[i].prefab, position, Quaternion.identity);
            }
        }
    }
}
