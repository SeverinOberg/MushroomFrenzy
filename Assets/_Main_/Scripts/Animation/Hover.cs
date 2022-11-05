using DG.Tweening;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private float offset = 0.5f;
    [SerializeField] private float duration = 2;

    private void OnEnable()
    {
        transform.DOMoveY(transform.position.y + offset, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
