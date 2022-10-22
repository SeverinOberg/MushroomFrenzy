using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Pickup : MonoBehaviour 
{
    private Collider2D collision;

    public enum Type {wood, stone, ironOre, ironBar};

    public Type type;
    [SerializeField] private int amount;
    [SerializeField] private float destroyAfter = 120;

    public bool      animate = true;
    public Direction spawnDirection;
    public float     spawnForce = 1f;
    

    private void Awake()
    {
        collision = GetComponent<Collider2D>();
    }

    private void Start()
    {
        Utilities.DestroyAfterDelay(gameObject, destroyAfter);
        if (animate)
        {
            DoJumpInDirection(spawnDirection);
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

    private void DoJumpInDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                JumpInDirection(Random.Range(-2f, -3f), Random.Range(-1f, 1f)) ;
                break;
            case Direction.Up:
                JumpInDirection(Random.Range(-1f, 1f), Random.Range(1.5f, 3f));
                break;
            case Direction.Right:
                JumpInDirection(Random.Range(2f, 3f), Random.Range(-1f, 1f));
                break;
            case Direction.Down:
                JumpInDirection(Random.Range(-1f, 1f), Random.Range(-1.5f, -3f));
                break;
            default:
                float offsetX = Random.Range(0, 1 + 1) > 0 ? Random.Range(1f, 2f) : Random.Range(-1f, -2f);
                float offsetY = Random.Range(0, 1 + 1) > 0 ? Random.Range(1f, 2f) : Random.Range(-1f, -2f);
                JumpInDirection(offsetX, offsetY);
                break;
        }
    }

    private void JumpInDirection(float offsetX, float offsetY)
    {
         transform.DOJump(new Vector2(transform.position.x + offsetX * spawnForce, transform.position.y + offsetY * spawnForce), 2, 1, 1f).SetEase(Ease.Flash)
        .OnComplete(() =>
        {
            transform.DOShakeRotation(0.3f, new Vector3(0, 0, 45));
            transform.DOShakeScale(1f);
            collision.enabled = true;
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

}
