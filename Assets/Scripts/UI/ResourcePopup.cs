using UnityEngine;
using DG.Tweening;
using TMPro;


public class ResourcePopup : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float fadeDuration;
    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;

    private int    amount;
    private string resourceName;
    private bool   isIncrease;

    public void Execute(int amount, string resourceName, bool isIncrease, Transform parent)
    {
        ResourcePopup popup = Instantiate(gameObject, parent, false).GetComponent<ResourcePopup>();
        popup.amount        = amount;
        popup.resourceName  = resourceName;
        popup.isIncrease    = isIncrease;
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void Start()
    {
        if (isIncrease)
        {
            textMesh.color = positiveColor;
            textMesh.text = $"+{amount} {resourceName}";
        }
        else
        {
            textMesh.color = negativeColor;
            textMesh.text = $"-{amount} {resourceName}";
        }

        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveX(120, 0.2f).SetEase(Ease.Flash));
        seq.Append(transform.DOLocalMoveY(-230, 2f).SetEase(Ease.Flash));
        seq.Append(textMesh.DOFade(0, fadeDuration).OnComplete(() => { Destroy(gameObject); } ));
        seq.Play();
    }

}

