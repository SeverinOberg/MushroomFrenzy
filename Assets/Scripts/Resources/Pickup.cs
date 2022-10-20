using UnityEngine;
using DG.Tweening;

public class Pickup : MonoBehaviour 
{
    private Collider2D collision;

    public enum Type {wood, stone, ironOre, ironBar};

    public Type type;
    [SerializeField] private int amount;

    public bool animate = true;

    private void Awake()
    {
        collision = GetComponent<Collider2D>();
    }

    private void Start()
    {
        Utilities.DestroyAfterDelay(gameObject, 120);
        if (animate)
        {
            float offsetX = Random.Range(0, 1 + 1) > 0 ? Random.Range(1f, 2f) : Random.Range(-1f, -2f);
            float offsetY = Random.Range(0, 1 + 1) > 0 ? Random.Range(1f, 2f) : Random.Range(-1f, -2f);
            transform.DOJump(new Vector2(transform.position.x + offsetX, transform.position.y + offsetY), 2, 1, 0.5f).SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                transform.DOShakeRotation(0.3f, new Vector3(0, 0, 45));
                transform.DOShakeScale(1f);
                collision.enabled = true;
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!collision.TryGetComponent(out ResourceManager resourceManager))
            {
                Debug.LogError("could not find Resource Manager on Player, this is unexpected");
                return;
            }

            switch (type)
            {
                case Type.wood:
                    resourceManager.Wood += amount;
                    break;
                case Type.stone:
                    resourceManager.Stone += amount;
                    break;
                case Type.ironOre:
                    resourceManager.IronOre += amount;
                    break;
                case Type.ironBar:
                    resourceManager.IronBar += amount;
                    break;
                default:
                    Debug.LogError("Could not find this Resource type, something went wrong");
                    return;
            }

            transform.DOKill();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

}
