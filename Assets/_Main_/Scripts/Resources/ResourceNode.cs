using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using DG.Tweening;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class ResourceNodeData
{
    public ResourceNode.ResourceType type;
    public Sprite sprite;

    public ResourceNodeData(ResourceNode.ResourceType type, Sprite sprite)
    {
        this.type   = type;
        this.sprite = sprite;
    }
}

public class ResourceNode : MonoBehaviour
{

    [SerializeField] private Collider2D collision;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject shadow;
    [SerializeField] private Material pureWhiteMaterial;

    public enum ResourceType
    {
        Wood,
        Stone,
        IronOre,
    }

    private ResourceType type;
    
    public ResourceType Type { get { return type; } }

    private int GatheredTimes;
    private int GatherLimit;
    private int minGatherLimit = 2;
    private int maxGatherLimit = 5;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        gameObject.SetActive(false);
    }

    // Initialize a disabled resource node
    public void Initialize(ResourceNodeData data)
    {
        type = data.type;
        spriteRenderer.sprite = data.sprite;

        GatheredTimes = 0;
        GatherLimit = Random.Range(minGatherLimit, maxGatherLimit);
        
        gameObject.SetActive(true);
        transform.DOPunchScale(originalScale * 0.2f, 1);
    }

    // Gather spawns this type's pickup resource
    public void Gather()
    {
        switch (type)
        {
            case ResourceType.Wood:
                SpawnResourcePickup(PickupDatabase.Instance.Get(Pickup.Type.Wood));
                break;
            case ResourceType.Stone:
                SpawnResourcePickup(PickupDatabase.Instance.Get(Pickup.Type.Stone));
                break;
            case ResourceType.IronOre:
                SpawnResourcePickup(PickupDatabase.Instance.Get(Pickup.Type.IronOre));
                break;
            default:
                Debug.LogError("unknown gather type");
                break;
        }

        BlinkMaterial(pureWhiteMaterial);
        transform.DOPunchScale(Vector2.one * 0.1f, 1);
        transform.DOPunchPosition(Vector2.up * 0.25f, 1);
        GatheredTimes++;
        if (GatheredTimes >= GatherLimit)
        {
            ResourceNodeManager.OnNodeGathered?.Invoke();
            transform.DOScale(0, 1);
            spriteRenderer.DOColor(Color.gray, 1).OnComplete(() => 
            {
                gameObject.SetActive(false);
                //transform.localScale = originalScale;
                spriteRenderer.color = Color.white;
            });
        }
    }

    private void SpawnResourcePickup(GameObject prefab)
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private void BlinkMaterial(Material pureWhiteMaterial, float seconds = 0.075f)
    {
        Material priorMaterial = spriteRenderer.material;
        StartCoroutine(DoBlinkMaterial(pureWhiteMaterial, priorMaterial, seconds));
    }

    private IEnumerator DoBlinkMaterial(Material pureWhiteMaterial, Material priorMaterial, float seconds)
    {
        spriteRenderer.material = pureWhiteMaterial;
        yield return new WaitForSeconds(seconds);
        spriteRenderer.material = priorMaterial;
    }

}
