using UnityEngine;
using UnityEngine.EventSystems;
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

    private Building selectedTarget;

    private float timeSinceLastDataUpdate;
    private float dataUpdateCooldown = 0.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && selectedTarget && !BuildingSystem.Instance.buildMode)
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
            }
        }
    }

    private void SelectTarget()
    {
        PointerEventData pointerEventData = new(EventSystem.current);
        if (pointerEventData.selectedObject)
        {
            return;
        }

        if (Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] hits))
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.TryGetComponent(out selectedTarget))
                {
                    selector.SetActive(true);
                    selector.transform.localScale = selectedTarget.GetComponent<BoxCollider2D>().size;
                    DisplaySelectedInterface(true);
                    break;
                }
            }
        }
    }

    private void ClearSelection()
    {
        selector.SetActive(false);
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
        selectedTarget.UpgradeBuilding();
        InjectHoverTooltipData("Upgrade", GetUpgradeData());
    }

    public void Repair()
    {
        selectedTarget.RepairBuilding();
    }

    public void Sell()
    {
        selectedTarget.SellBuilding();
        ClearSelection();
    }

    public void OnMouseEnter()
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
                    InjectHoverTooltipData("Upgrade", GetUpgradeData());
                    break;
                case "UI Repair":
                    InjectHoverTooltipData("Repair", GetRepairData());
                    break;
                case "UI Sell":
                    InjectHoverTooltipData("Sell", GetSellData());
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

    private string GetUpgradeData()
    {
        switch (selectedTarget.Level)
        {
            case 1:
                if (selectedTarget.Level >= selectedTarget.MaxLevel)
                {
                    return $"This building is already fully upgraded! Nice!";
                }
                return $"Costs: Wood: {selectedTarget.buildingData.level2UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingData.level2UpgradeStoneCost}" +
                       $" - Metal: {selectedTarget.buildingData.level2UpgradeMetalCost}";
            case 2:
                return $"Costs: Wood: {selectedTarget.buildingData.level3UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingData.level3UpgradeStoneCost}" +
                       $" - Metal: {selectedTarget.buildingData.level3UpgradeMetalCost}";
            case 3:
                return $"This building is already fully upgraded! Nice!";
            default:
                return "Unknown hover tooltip";
        }
    }

    // @TODO:
    private string GetRepairData()
    {
        switch (selectedTarget.Level)
        {
            case 1:
                if (selectedTarget.Level >= selectedTarget.MaxLevel)
                {
                    return $"This building is already fully upgraded! Nice!";
                }
                return $"Costs: Wood: {selectedTarget.buildingData.level2UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingData.level2UpgradeStoneCost}" +
                       $" - Metal: {selectedTarget.buildingData.level2UpgradeMetalCost}";
            case 2:
                return $"Costs: Wood: {selectedTarget.buildingData.level3UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingData.level3UpgradeStoneCost}" +
                       $" - Metal: {selectedTarget.buildingData.level3UpgradeMetalCost}";
            case 3:
                return $"This building is already fully upgraded! Nice!";
            default:
                return "Unknown hover tooltip";
        }
    }

    // @TODO:
    private string GetSellData()
    {
        switch (selectedTarget.Level)
        {
            case 1:
                if (selectedTarget.Level >= selectedTarget.MaxLevel)
                {
                    return $"This building is already fully upgraded! Nice!";
                }
                return $"Costs: Wood: {selectedTarget.buildingData.level2UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingData.level2UpgradeStoneCost}" +
                       $" - Metal: {selectedTarget.buildingData.level2UpgradeMetalCost}";
            case 2:
                return $"Costs: Wood: {selectedTarget.buildingData.level3UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingData.level3UpgradeStoneCost}" +
                       $" - Metal: {selectedTarget.buildingData.level3UpgradeMetalCost}";
            case 3:
                return $"This building is already fully upgraded! Nice!";
            default:
                return "Unknown hover tooltip";
        }
    }

    public void OnMouseExit()
    {
        hoverTooltip.SetActive(false);
    }

}
