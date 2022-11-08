using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Collider2D     _collision;
    [SerializeField] private GameObject     body;
    [SerializeField] private GameObject     shadow;
    [SerializeField] private AudioClip      pickupClip;
    [SerializeField] private ParticleSystem impactPS;
    [SerializeField] public int amount;
    [SerializeField] private float destroyAfter = 120;
    
    public enum Type {SpiritEssence, Wood, Stone, IronOre, IronBar};

    public Type type;

    public bool      animate = true;
    public Direction spawnDirection;
    public float     spawnForce = 1f;

    private void Start()
    {
        Utilities.DestroyAfterDelay(gameObject, destroyAfter);
        if (animate)
        {
            DoJumpInDirection(spawnDirection);
        }
        else
        {
            _collision.enabled = true;
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

            AudioSource.PlayClipAtPoint(pickupClip, transform.position);

            body.SetActive(false);
            shadow.SetActive(false);
            _collision.enabled = false;

            List<PopupData> popupData = new List<PopupData>();
            popupData.Add(new (amount, GetNameStringFromType(), true));
            resourceManager.popup.Execute(popupData);

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
            _collision.enabled = true;
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    private string GetNameStringFromType()
    {
        switch (type)
        {
            case Type.SpiritEssence:
                return "Spirit Essence";
            case Type.Wood:
                return "Wood";
            case Type.Stone:
                return "Stone";
            case Type.IronOre:
                return "Iron Ore";
            case Type.IronBar:
                return "Iron Bar";
            default:
                return "Unknown";
        }
    }

}
