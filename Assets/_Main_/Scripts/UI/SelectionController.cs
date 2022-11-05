using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SelectionController : MonoBehaviour
{

    private PlayerController player;

    [SerializeField] private GameObject selectedTargetUI;
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
    [SerializeField] private Image sellButton;

    private Color originalButtonColor;

    private Building selectedTarget;

    private float timeSinceLastDataUpdate;
    private float dataUpdateCooldown = 0.25f;

    private bool hoveringSellButton;

    private PointerEventData pointerEventData;

    public static System.Action<Building> OnSelectTarget;
    public static System.Action OnClearSelection;

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();

        timeSinceLastDataUpdate = dataUpdateCooldown;
        originalButtonColor = upgradeButton.color;
        repairButton.color = Utilities.RedColor;
    }
    
    private void OnEnable()
    {
        OnClearSelection += ClearSelection;
    }

    private void OnDisable()
    {
        OnClearSelection -= ClearSelection;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape) && selectedTarget && !player.BuildingSystem.BuildMode)
        {
            ClearSelection();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject() && !player.BuildingSystem.BuildMode)
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
                    selector.transform.localScale = selectedTarget.SelectionSize;
                    DisplaySelectedInterface(true);
                    OnSelectTarget?.Invoke(target);

                    if (selectedTarget.UnitType == Unit.UnitTypes.Base)
                    {
                        sellButton.color = Utilities.RedColor;
                    }
                    else
                    {
                        sellButton.color = originalButtonColor;
                    }
  

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
            selectedTargetUI.SetActive(true);
        }
        else
        {
            selectedTargetUI.SetActive(false);
        }
    }

    private void InjectTargetDataIntoInterface()
    {
        title.text = selectedTarget.Title;
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

        healthbar.localScale = new Vector2
        (
            Utilities.CalculatePercentageNormalized(selectedTarget.Health, selectedTarget.MaxHealth),
            healthbar.localScale.y
        );
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
        if (selectedTarget.UnitType == Unit.UnitTypes.Base)
        {
            UIManager.LogToScreen("You can't sell your own base...");
            return; 
        }
            

        if (selectedTarget.SellBuilding())
        {
            ClearSelection();
        }
    }

    public void OnMouseEnterButton()
    {
        hoverTooltip.SetActive(true);
        
        pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
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
                return $"Wood: {selectedTarget.buildingSO.level2UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingSO.level2UpgradeStoneCost}" +
                       $" - Iron Bars: {selectedTarget.buildingSO.level2UpgradeIronBarCost}";
            case 2:
                return $"Wood: {selectedTarget.buildingSO.level3UpgradeWoodCost}" +
                       $" - Stone: {selectedTarget.buildingSO.level3UpgradeStoneCost}" +
                       $" - Iron Bars: {selectedTarget.buildingSO.level3UpgradeIronBarCost}";
            case 3:
                return $"This building is already fully upgraded! Nice!";
            default:
                return "Unknown hover tooltip";
        }
    }

    private string GetRepairInput()
    {
        ResourceObject repairDataByLevel = player.ResourceManager.GetRepairDataByLevel(selectedTarget.buildingSO, selectedTarget.Level);
        return
        (
            $"Wood: {repairDataByLevel.wood}" +
            $" - Stone: {repairDataByLevel.stone}" +
            $" - Iron Bars: {repairDataByLevel.ironBar}"
        );
    }

    private string GetSellInput()
    {
        ResourceObject sellDataByLevel;

        if (selectedTarget.Health < selectedTarget.MaxHealth)
        {
            sellDataByLevel = player.ResourceManager.GetSellDataByLevel(selectedTarget.buildingSO, selectedTarget.Level, damaged: true);
        }
        else
        {
            sellDataByLevel = player.ResourceManager.GetSellDataByLevel(selectedTarget.buildingSO, selectedTarget.Level, damaged: false);
        }

        return $"Wood: {sellDataByLevel.wood}" +
               $" - Stone: {sellDataByLevel.stone}" +
               $" - Iron Bars: {sellDataByLevel.ironBar}";
    }

    public void OnMouseExitButton()
    {
        if (hoveringSellButton)
            hoveringSellButton = false;

        hoverTooltip.SetActive(false);
    }

}
