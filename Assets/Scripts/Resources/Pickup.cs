using UnityEngine;
using DG.Tweening;

public class Pickup : MonoBehaviour 
{

    private enum TypeOfResource {wood, stone, ironOre, ironBar};

    [SerializeField] private TypeOfResource typeOfResource;
    [SerializeField] private int amount;

    public bool animate = true;

    private void Start()
    {
        Utilities.DestroyAfterDelay(gameObject, 120);
        if (animate)
        {
            transform.DOJump(new Vector2(transform.position.x + Random.Range(-2, 2), transform.position.y), 2, 1, 1).SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                transform.DOShakeRotation(0.3f, new Vector3(0, 0, 45));
                transform.DOShakeScale(1f);
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

            switch (typeOfResource)
            {
                case TypeOfResource.wood:
                    resourceManager.Wood += amount;
                    break;
                case TypeOfResource.stone:
                    resourceManager.Stone += amount;
                    break;
                case TypeOfResource.ironOre:
                    resourceManager.IronOre += amount;
                    break;
                case TypeOfResource.ironBar:
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
