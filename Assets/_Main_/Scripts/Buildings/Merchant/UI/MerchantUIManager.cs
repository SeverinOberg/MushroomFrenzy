using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MerchantUIManager : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] private PlayerController     player;
    [SerializeField] private ResourceManager      resourceManager;
    [SerializeField] private TextMeshProUGUI      selectedItemText;
    [SerializeField] private Button               buyButton;
    [SerializeField] private Button               quantityDecreaseButton;
    [SerializeField] private Button               quantityIncreaseButton;
    [SerializeField] private Button               resetQuantityButton;

    [SerializeField] private RectTransform        quantityDecreaseIcon;
    [SerializeField] private RectTransform        quantityIncreaseIcon;
    [SerializeField] private RectTransform        resetQuantityIcon;

    [SerializeField] private TMP_InputField       quantityInputField;
    [SerializeField] private List<MerchantItemUI> merchantItems;
    
    private ItemSO selectedItem;
    private int quantity = 1;

    private void OnEnable()
    {
        player.InputController.OnCancel += player.UIManager.DeactivateMerchantUI;

        for (int i = 0; i < merchantItems.Count; i++)
        {
            merchantItems[i].OnSelectMerchantItem += SetSelectedItem;
        }

        buyButton.onClick.AddListener(OnBuyButtonClickCallback);
        quantityDecreaseButton.onClick.AddListener(OnQuantityDecreaseButtonClickCallback);
        quantityIncreaseButton.onClick.AddListener(OnQuantityIncreaseButtonClickCallback);
        resetQuantityButton.onClick.AddListener(OnResetQuantityButtonClickCallback);
    }

    private void OnDisable()
    {
        player.InputController.OnCancel += player.UIManager.DeactivateMerchantUI;

        selectedItem = null;
        for (int i = 0; i < merchantItems.Count; i++)
        {
            merchantItems[i].OnSelectMerchantItem -= SetSelectedItem;
        }

        buyButton.onClick.RemoveAllListeners();
        quantityDecreaseButton.onClick.RemoveAllListeners();
        quantityIncreaseButton.onClick.RemoveAllListeners();
        resetQuantityButton.onClick.RemoveAllListeners();
    }

    private void SetSelectedItem(ItemSO merchantItem)
    {
        if (selectedItem == merchantItem)
            return;

        selectedItem          = merchantItem;
        UpdateSelectedItemText();

        if (!buyButton.interactable)
        {
            buyButton.interactable              = true;
            quantityDecreaseButton.interactable = true;
            quantityIncreaseButton.interactable = true;
            resetQuantityButton.interactable    = true;
        }
    }

    private void OnBuyButtonClickCallback()
    {
        ResourceObject payload = new ResourceObject(selectedItem.buyPrice.spiritEssence * quantity);
        if (!resourceManager.HasSufficientResourcesToBuy(payload))
        { 
            // Negative animation, maybe on Buy button? Like a shake or red color etc
            return;
        }

        IPurchaseable purchaseInterface = selectedItem.prefab.GetComponent<IPurchaseable>();

        if (!purchaseInterface.Validate(player, quantity))
        {
            return;
        }

        purchaseInterface.Purchase(player, quantity);
        resourceManager.DecreaseResources(payload);
    }

    private void OnQuantityDecreaseButtonClickCallback()
    {
        if (quantity <= 1)
        {
            return;
        }

        quantityDecreaseIcon.DOComplete();
        quantityDecreaseIcon.DOPunchPosition(Vector3.left * 1.5f, 0.25f);
        quantityDecreaseIcon.DOPunchScale(Vector3.one  * 0.5f, 0.25f);

        quantity -= 1;
        quantityInputField.text = $"{quantity}";
        UpdateSelectedItemText();
    }

    private void OnQuantityIncreaseButtonClickCallback()
    {
        if (quantity >= 100)
        {
            return;
        }

        quantityIncreaseIcon.DOComplete();
        quantityIncreaseIcon.DOPunchPosition(Vector3.right * 1.5f, 0.25f);
        quantityIncreaseIcon.DOPunchScale(Vector3.one * 0.5f, 0.25f);

        quantity += 1;
        quantityInputField.text = $"{quantity}";
        UpdateSelectedItemText();
    }

    private void OnResetQuantityButtonClickCallback()
    {
        if (quantity <= 1)
        {
            return;
        }

        resetQuantityIcon.DORotate(
            endValue: new Vector3(transform.rotation.x, transform.rotation.y, -360),
            duration: 1,
            mode:     RotateMode.FastBeyond360).SetEase(Ease.OutBounce);

        quantity = 1;
        quantityInputField.text = $"{quantity}";
        UpdateSelectedItemText();
    }

    private void UpdateSelectedItemText()
    {
        selectedItemText.text = $"{selectedItem.title} - {selectedItem.buyPrice.spiritEssence * quantity}";
    }

}
