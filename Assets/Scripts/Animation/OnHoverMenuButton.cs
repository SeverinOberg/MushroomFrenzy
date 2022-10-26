using DG.Tweening;
using UnityEngine;

public class OnHoverMenuButton : MonoBehaviour
{
    [SerializeField] private Vector2 offset;
    [SerializeField] private float duration;

    private Vector2 defaultPosition;
    private Vector2 offsetPosition;

    private void Awake()
    {
        defaultPosition = transform.position;
        offsetPosition  = new Vector2(transform.position.x + offset.x, transform.position.y + offset.y) ;
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public void OnMouseEnter()
    {
        transform.DOMove(offsetPosition, duration).SetEase(Ease.InSine);
    }

    public void OnMouseExit()
    {
        transform.DOMove(defaultPosition, duration).SetEase(Ease.OutSine);
    }

}
