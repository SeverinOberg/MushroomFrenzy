using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{

    public static ItemDatabase Instance;

    [SerializeField] private List<ItemSO> items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public ItemSO Get(string title)
    {
        return items.Find((item) => item.title == title);
    }

}
