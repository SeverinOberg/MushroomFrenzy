using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SelectionController : MonoBehaviour
{

    [SerializeField] private GameObject selectedTargetInterface;
    [SerializeField] private GameObject hoverTooltip;
    [SerializeField] private GameObject selector;

    [SerializeField] private TextMeshProUGUI hoverTitle;
    [SerializeField] private TextMeshProUGUI hoverBody;


    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private RectTransform   healthbar;

    [SerializeField] private Image upgradeButton;
    [SerializeField] private Image repairButton;

    private Color originalButtonColor;

    private Building selectedTarget;

    private float timeSinceLastDataUpdate;
    private float dataUpdateCooldown = 0.5f;

    private bool hoveringSellButton;

    private void Awake()
    {
        originalButtonColor = upgradeButton.color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape) && selectedTarget && !BuildingSystem.Instance.buildMode)
        {
            ClearSelection();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !BuildingSystem.Instance.buildMode)
        {
            SelectTarget();
        }

        if (selectedTarget)
        {
            if (selectedTarget.IsDead)
            {
                ClearSelection();
                return;
            }

            selector.transform.position = selectedTarget.transform.position;
            
            timeSinceLastDataUpdate += Time.deltaTime;
            if (timeSinceLastDataUpdate > dataUpdateCooldown)
            {
                timeSinceLastDataUpdate = 0;
                UpdateTargetHealth();
                UpdateTargetLevel();

                repairButton.color = selectedTarget.Health >= selectedTarget.MaxHealth ? Utilities.RedColor : originalButtonColor;
                if (hoveringSellButton)
                {
                    InjectHoverTooltipData("Sell", GetSellInput());
                }
            }
        }
    }

    private void SelectTarget()
    {
        if (Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] hits))
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Building target = hits[i].transform.GetComponent<Building>();
                if (target && !target.IsDead)
                {
                    selectedTarget = target;
                    selector.SetActive(true);
                    selector.transform.localScale = selectedTarget.GetComponent<BoxCollider2D>().size;
                    DisplaySelectedInterface(true);

                    if (selectedTarget.Level >= selectedTarget.MaxLevel)
                    {
                        upgradeButton.color = Utilities.RedColor;
                    }
                    else
                    {
                        upgradeButton.color = originalButtonColor;
                    }

                    break;
                }
            }
        }
    }

    public void ClearSelection()
    {
        selector.SetActive(false);
        hoverTooltip.SetActive(false);
        DisplaySelectedInterface(false);
        selectedTarget = null;
    }

    private void DisplaySelectedInterface(bool displayInterface)
    {
        if (displayInterface)
        {
            InjectTargetDataIntoInterface();
            selectedTargetInterface.SetActive(true);
        }
        else
        {
            selectedTargetInterface.SetActive(false);
        }
    }

    private void InjectTargetDataIntoInterface()
    {
        title.text = selectedTarget.unitData.title;
        UpdateTargetLevel();
        UpdateTargetHealth();
    }

    private void UpdateTargetLevel()
    {
        level.text = $"Level: {selectedTarget.Level} / {selectedTarget.MaxLevel}";
    }

    private void UpdateTargetHealth()
    {
        health.text = $"{(int)selectedTarget.Health} / {(int)selectedTarget.MaxHealth}";
        healthbar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Utilities.CalculatePercentageNormalized(selectedTarget.MaxHealth, selectedTarget.Health));
    }

    public void Upgrade()
    {
        if (selectedTarget.UpgradeBuilding())
        {
            if (selectedTarget.Level >= selectedTarget.MaxLevel)
                upgradeButton.color = Utilities.RedColor;

            InjectHoverTooltipData("Upgrade", GetUpgradeInput());
        }
    }

    public void Repair()
    {
        if (selectedTarget.RepairBuilding())
        {
            InjectHoverTooltipData("Repair", GetRepairInput());
        }
    }

    public void Sell()
    {
        if (selectedTarget.SellBuilding())
        {
            ClearSelection();
        }
    }

    public void OnMouseEnterButton()
    {
        hoverTooltip.SetActive(true);
        
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        for (int i = 0; i < results.Count; i++)
        {
            switch (results[i].gameObject.tag)
            {
                case "UI Upgrade":
                    InjectHoverTooltipData("Upgrade", GetUpgradeInput());
                    break;
                case "UI Repair":
                    InjectHoverTooltipData("Repair", GetRepairInput());
                    break;
                case "UI Sell":
                    InjectHoverTooltipData("Sell", GetSellInput());
                    hoveringSellButton = true;
                    break;
                default:
                    continue;
            }
        }
    }

    private void InjectHoverTooltipData(string title, string body)
    {
        hoverTitle.text = title;
        hoverBody.text  = body;
    }

    private string GetUpgradeInput()
    {
        switch (selectedTarget.Level)
        {
            case 1:
                if (selectedTarget.Level >= selectedTarget.MaxLevel)
                {
                    return $"This building is already fully upgraded! Nice!";
                }
                return $"Wood: {selectedTarget.buildingData.level2UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingData.level2UpgradeStoneCost}" +
                       $" - Metal: {selectedTarget.buildingData.level2UpgradeMetalCost}";
            case 2:
                return $"Wood: {selectedTarget.buildingData.level3UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingData.level3UpgradeStoneCost}" +
                       $" - Metal: {selectedTarget.buildingData.level3UpgradeMetalCost}";
            case 3:
                return $"This building is already fully upgraded! Nice!";
            default:
                return "Unknown hover tooltip";
        }
    }

    private string GetRepairInput()
    {
        ResourceDataObject repairDataByLevel = ResourceManager.Instance.GetRepairDataByLevel(selectedTarget.buildingData, selectedTarget.Level);
        return
        (
            $"Wood: {repairDataByLevel.wood}" +
            $" - Stone: {repairDataByLevel.stone}" +
            $" - Metal: {repairDataByLevel.metal}"
        );
    }

    private string GetSellInput()
    {
        ResourceDataObject sellDataByLevel;

        if (selectedTarget.Health < selectedTarget.MaxHealth)
        {
            sellDataByLevel = ResourceManager.Instance.GetSellDataByLevel(selectedTarget.buildingData, selectedTarget.Level, damaged: true);
        }
        else
        {
            sellDataByLevel = ResourceManager.Instance.GetSellDataByLevel(selectedTarget.buildingData, selectedTarget.Level, damaged: false);
        }

        return $"Wood: {sellDataByLevel.wood}" +
               $" - Stone: {sellDataByLevel.stone}" +
               $" - Metal: {sellDataByLevel.metal}";
    }

    public void OnMouseExitButton()
    {
        if (hoveringSellButton)
            hoveringSellButton = false;

        hoverTooltip.SetActive(false);
    }

}
