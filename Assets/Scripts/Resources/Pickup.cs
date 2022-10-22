using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class Pickup : MonoBehaviour 
{
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject shadow;
    [SerializeField] private ParticleSystem impactPS;
    [SerializeField] private int amount;
    [SerializeField] private float destroyAfter = 120;


    private Collider2D collision;

    public enum Type {SpiritEssence, Wood, Stone, IronOre, IronBar};

    public Type type;


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
        else
        {
            collision.enabled = true;
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
                case Type.SpiritEssence:
                    resourceManager.SpiritEssence += amount;
                    break;
                case Type.Wood:
                    resourceManager.Wood += amount;
                    break;
                case Type.Stone:
                    resourceManager.Stone += amount;
                    break;
                case Type.IronOre:
                    resourceManager.IronOre += amount;
                    break;
                case Type.IronBar:
                    resourceManager.IronBar += amount;
                    break;
                default:
                    Debug.LogError("Could not find this Resource type, something went wrong");
                    return;
            }

            transform.DOKill();

            if (impactPS)
                impactPS.Play();


            body.SetActive(false);
            shadow.SetActive(false);
            Destroy(gameObject, 2f);
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
            transform.DOPunchScale(Vector2.one * 0.5f, 0.5f);
            collision.enabled = true;
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

}
