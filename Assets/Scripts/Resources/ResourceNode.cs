using UnityEngine;

[System.Serializable]
public class ResourceNodeData
{
    public ResourceNode.Type type;
    public Sprite sprite;

    public ResourceNodeData(ResourceNode.Type type, Sprite sprite)
    {
        this.type   = type;
        this.sprite = sprite;
    }
}

public class ResourceNode : MonoBehaviour
{

    public enum Type
    {
        wood,
        stone,
        ironOre,
    }

    private Type type;
    private SpriteRenderer spriteRenderer;

    private int GatheredTimes;
    private int GatherLimit;
    private int minGatherLimit = 2;
    private int maxGatherLimit = 5;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    // Initialize a disabled resource node
    public void Initialize(ResourceNodeData data)
    {
        this.type = data.type;
        spriteRenderer.sprite = data.sprite;

        GatheredTimes = 0;
        GatherLimit = Random.Range(minGatherLimit, maxGatherLimit);
        
        gameObject.SetActive(true);
    }

    // Gather spawns this type's pickup resource
    public void Gather()
    {
        switch (type)
        {
            case Type.wood:
                SpawnResourcePickup(PickupDatabase.Instance.Get(Pickup.Type.Wood));
                break;
            case Type.stone:
                SpawnResourcePickup(PickupDatabase.Instance.Get(Pickup.Type.Stone));
                break;
            case Type.ironOre:
                SpawnResourcePickup(PickupDatabase.Instance.Get(Pickup.Type.IronOre));
                break;
            default:
                Debug.LogError("unknown gather type");
                break;
        }
        GatheredTimes++;
        if (GatheredTimes >= GatherLimit)
        {
            ResourceNodeManager.OnNodeGathered?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void SpawnResourcePickup(GameObject prefab)
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }

}
