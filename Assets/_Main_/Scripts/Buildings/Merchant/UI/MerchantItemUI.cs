using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MerchantItemUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{

    [Header("Essentials")]
    [Header("Merchant UI")]
    [SerializeField] private Image iconImage;

    [Header("Tooltip UI")]
    [SerializeField] private GameObject      tooltipParent;
    [SerializeField] private TextMeshProUGUI tooltipTitle;
    [SerializeField] private TextMeshProUGUI tooltipCost;
    [SerializeField] private TextMeshProUGUI tooltipDescription;

    [Header("Variation")]
    [SerializeField] private ItemSO merchantItem;

    public System.Action<ItemSO> OnSelectMerchantItem;

    private void Awake()
    {
        if (merchantItem.icon)
            iconImage.sprite = merchantItem.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipTitle.text = merchantItem.title;
        tooltipCost.text = $"{merchantItem.buyPrice.spiritEssence}";
        tooltipDescription.text = merchantItem.description;

        tooltipParent.SetActive(true);
        tooltipParent.transform.DOScale(1, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipParent.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            if (!eventData.pointerCurrentRaycast.gameObject)
            {
                tooltipParent.SetActive(false);
            }
        });
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.worldPosition == Vector3.zero) 
            return;

        tooltipParent.transform.position = eventData.pointerCurrentRaycast.worldPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelectMerchantItem?.Invoke(merchantItem);
    }

}
