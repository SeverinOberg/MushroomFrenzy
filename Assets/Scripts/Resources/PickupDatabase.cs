using System.Collections.Generic;
using UnityEngine;

public class PickupDatabase : MonoBehaviour
{
    public static PickupDatabase Instance;
    [SerializeField] private List<Pickup> prefabs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public GameObject Get(Pickup.Type type)
    {
        return Instance.prefabs.Find((pickup) => pickup.type == type).gameObject;
    }

}
