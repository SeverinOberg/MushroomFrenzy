using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour 
{
    public Popup popup;

    private int spiritEssence;
    private int wood;
    private int stone;
    private int ironOre;
    private int ironBar;

    public int SpiritEssence { get { return spiritEssence; } set { spiritEssence = value; OnSetSpiritEssence?.Invoke(spiritEssence); } }
    public int Wood          { get { return wood; }          set { wood          = value; OnSetWood?.Invoke(wood); } }
    public int Stone         { get { return stone; }         set { stone         = value; OnSetStone?.Invoke(stone); } }
    public int IronOre       { get { return ironOre; }       set { ironOre       = value; OnSetIronOre?.Invoke(ironOre); } }
    public int IronBar       { get { return ironBar; }       set { ironBar       = value; OnSetIronBar?.Invoke(ironBar); } }

    public Action<int> OnSetSpiritEssence;
    public Action<int> OnSetWood;
    public Action<int> OnSetStone;
    public Action<int> OnSetIronOre;
    public Action<int> OnSetIronBar;

    private void Start()
    {
        SpiritEssence = 2000;
        Wood          = 300;
        Stone         = 300;
        IronOre       = 100;
        IronBar       = 100;
    }

    private void BuildAndExecutePopups(ResourceObject resourceObject, bool isIncrease)
    {
        List<PopupData> popupData = new List<PopupData>();

        if (resourceObject.spiritEssence != 0)
            popupData.Add(new (resourceObject.spiritEssence, "Spirit Essence", isIncrease));
        if (resourceObject.wood != 0)
            popupData.Add(new (resourceObject.wood, "Wood", isIncrease));
        if (resourceObject.stone != 0)
            popupData.Add(new (resourceObject.stone, "Stone", isIncrease));
        if (resourceObject.ironOre != 0)
            popupData.Add(new (resourceObject.ironOre, "Iron Ore", isIncrease));
        if (resourceObject.ironBar != 0)
            popupData.Add(new (resourceObject.ironBar, "Iron Bar", isIncrease));

        if (popupData != null)
            popup.Execute(popupData);
    }


    public bool PayForBuild(BuildingSO buildingData)
    {
        if (!HasSufficientResourcesToBuild(buildingData, out ResourceObject resourceObject))
        {
            return false;
        }

        DecreaseResources(resourceObject);

        return true;
    }

    public void PayForUpgrade(ResourceObject resourceObject)
    {
        DecreaseResources(resourceObject);
    }

    public void PayForRepair(ResourceObject resourceObject)
    {
        DecreaseResources(resourceObject);
    }

    public void SellBuilding(ResourceObject resourceObject)
    {
        IncreaseResources(resourceObject);
    }

    public void IncreaseResources(ResourceObject resourceObject)
    {
        SpiritEssence += resourceObject.spiritEssence;
        Wood          += resourceObject.wood;
        Stone         += resourceObject.stone;
        IronOre       += resourceObject.ironOre;
        IronBar       += resourceObject.ironBar;

        BuildAndExecutePopups(resourceObject, true);
    }

    public void DecreaseResources(ResourceObject resourceObject)
    {
        SpiritEssence -= resourceObject.spiritEssence;
        Wood          -= resourceObject.wood;
        Stone         -= resourceObject.stone;
        IronOre       -= resourceObject.ironOre;
        IronBar       -= resourceObject.ironBar;

        BuildAndExecutePopups(resourceObject, false);
    }

    public bool HasSufficientResourcesToBuy(ResourceObject resourceObject)
    {
        if (SpiritEssence >= resourceObject.spiritEssence &&
            Wood          >= resourceObject.wood          &&
            Stone         >= resourceObject.stone         &&
            IronOre       >= resourceObject.ironOre       &&
            IronBar       >= resourceObject.ironBar)
        {
            return true;
        } 
        
        UIManager.LogToScreen($"Not enough resources to buy item");
        return false;
    }

    public bool HasSufficientResourcesToBuild(BuildingSO buildingData, out ResourceObject resourceObject)
    {
        resourceObject = null;
        if (SpiritEssence >= buildingData.spiritEssenceCost &&
            Wood          >= buildingData.woodCost          &&
            Stone         >= buildingData.stoneCost         &&
            IronOre       >= buildingData.ironOreCost       &&
            IronBar       >= buildingData.ironBarCost)
        {
            resourceObject = new
            (
                buildingData.spiritEssenceCost,
                buildingData.woodCost,
                buildingData.stoneCost,
                buildingData.ironOreCost,
                buildingData.ironBarCost
            );
            return true;
        } 
        
        UIManager.LogToScreen($"Not enough resources to build");
        return false;
    }

    public bool HasSufficientResourcesToUpgrade(BuildingSO buildingData, int level, out ResourceObject resourceCost)
    {
        resourceCost = null;

        switch (level)
        {
            case 1:
                if (SpiritEssence >= buildingData.level2UpgradeSpiritEssenceCost &&
                    Wood          >= buildingData.level2UpgradeWoodCost          &&
                    Stone         >= buildingData.level2UpgradeStoneCost         &&
                    IronOre       >= buildingData.level2UpgradeIronOreCost       &&
                    IronBar       >= buildingData.level2UpgradeIronBarCost)
                {
                    resourceCost = new
                    (
                        buildingData.level2UpgradeSpiritEssenceCost,
                        buildingData.level2UpgradeWoodCost,
                        buildingData.level2UpgradeStoneCost,
                        buildingData.level2UpgradeIronOreCost,
                        buildingData.level2UpgradeIronBarCost
                    );
                    return true;
                }
                break;
            case 2:
                if (SpiritEssence >= buildingData.level3UpgradeSpiritEssenceCost &&
                    Wood          >= buildingData.level3UpgradeWoodCost          &&
                    Stone         >= buildingData.level3UpgradeStoneCost         &&
                    IronOre       >= buildingData.level3UpgradeIronOreCost       &&
                    IronBar       >= buildingData.level3UpgradeIronBarCost)
                {
                    resourceCost = new
                    (
                        buildingData.level3UpgradeSpiritEssenceCost,
                        buildingData.level3UpgradeWoodCost,
                        buildingData.level3UpgradeStoneCost,
                        buildingData.level3UpgradeIronOreCost,
                        buildingData.level3UpgradeIronBarCost
                    );
                    return true;
                }
                    
                break;
            case 3:
                // Do nothing, this is the max level.
                return true;
            default:
                Debug.LogError("Checking for sufficient resources to upgrade failed, unknown level");
                break;
        }

        UIManager.LogToScreen($"Not enough resources to upgrade");
        return false;
    }

    // Repairing always costs at least 1 or half the price to build it (rounded away from zero)
    public bool HasSufficientResourcesToRepair(BuildingSO buildingData, int level, out ResourceObject repairCostData)
    {
        repairCostData = null;

        switch (level)
        {
            case 1:
                repairCostData = GetRepairDataByLevel(buildingData, 1);
                if (SpiritEssence >= repairCostData.spiritEssence && Wood >= repairCostData.wood && Stone >= repairCostData.stone && IronBar >= repairCostData.ironBar)
                {
                    return true;
                }
                break;
            case 2:
                repairCostData = GetRepairDataByLevel(buildingData, 2);
                if (SpiritEssence >= repairCostData.spiritEssence && Wood >= repairCostData.wood && Stone >= repairCostData.stone && IronBar >= repairCostData.ironBar)
                {
                    return true;
                }
                break;
            case 3:
                repairCostData = GetRepairDataByLevel(buildingData, 3);
                if (SpiritEssence >= repairCostData.spiritEssence && Wood >= repairCostData.wood && Stone >= repairCostData.stone && IronBar >= repairCostData.ironBar)
                {
                    return true;
                }
                break;
            default:
                Debug.LogError("Checking for sufficient resources to repair failed, unknown level");
                break;
        }

        UIManager.LogToScreen($"Not enough resources to repair");
        return false;
    }

    public ResourceObject GetRepairDataByLevel(BuildingSO buildingData, int level)
    {
        ResourceObject result = null;

        switch (level)
        {

            case 1:
                result = new
                (
                    (int)Math.Round(buildingData.spiritEssenceCost    * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.woodCost             * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.stoneCost            * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.ironOreCost          * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.ironBarCost          * 0.5f, MidpointRounding.AwayFromZero)
                );
                break;
            case 2:
                result = new
                (
                    (int)Math.Round(buildingData.level2UpgradeSpiritEssenceCost * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level2UpgradeWoodCost          * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level2UpgradeStoneCost         * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level2UpgradeIronOreCost       * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level2UpgradeIronBarCost       * 0.5f, MidpointRounding.AwayFromZero)
                );
                break;
            case 3:
                result = new
                (
                    (int)Math.Round(buildingData.level3UpgradeSpiritEssenceCost * 0.75f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level3UpgradeWoodCost          * 0.75f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level3UpgradeStoneCost         * 0.75f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level3UpgradeIronOreCost       * 0.75f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level3UpgradeIronBarCost       * 0.75f, MidpointRounding.AwayFromZero)
                );
                break;
            default:
                Debug.LogError("Checking for sufficient resources to repair failed, unknown level");
                break;
        }

        return result;
    }

    // If the building has taken damage, the resources returned will be halfed. Takes the lower half of a number, NOT away from zero.
    // A building that costs 1 if taken damage will recieve 0 money back.
    public ResourceObject GetSellDataByLevel(BuildingSO buildingData, int level, bool damaged)
    {
        ResourceObject sellResourceData = null;

        if (damaged)
        {
            switch (level)
            {
                case 1:
                    sellResourceData = new
                    (
                        (int)(buildingData.spiritEssenceCost    * 0.5f),
                        (int)(buildingData.woodCost             * 0.5f),
                        (int)(buildingData.stoneCost            * 0.5f),
                        (int)(buildingData.ironOreCost          * 0.5f),
                        (int)(buildingData.ironBarCost          * 0.5f)
                    );
                    break;
                case 2:
                    sellResourceData = new
                    (
                        (int)(buildingData.spiritEssenceCost * 0.5f) + (int)(buildingData.level2UpgradeSpiritEssenceCost  * 0.5f),
                        (int)(buildingData.woodCost          * 0.5f) + (int)(buildingData.level2UpgradeWoodCost           * 0.5f),
                        (int)(buildingData.stoneCost         * 0.5f) + (int)(buildingData.level2UpgradeStoneCost          * 0.5f),
                        (int)(buildingData.ironOreCost       * 0.5f) + (int)(buildingData.level2UpgradeIronOreCost        * 0.5f),
                        (int)(buildingData.ironBarCost       * 0.5f) + (int)(buildingData.level2UpgradeIronBarCost        * 0.5f)
                    );
                    break;
                case 3:
                    sellResourceData = new
                    (
                        (int)(buildingData.spiritEssenceCost * 0.5f) + (int)(buildingData.level2UpgradeSpiritEssenceCost * 0.5f) + (int)(buildingData.level3UpgradeSpiritEssenceCost * 0.5f),
                        (int)(buildingData.woodCost          * 0.5f) + (int)(buildingData.level2UpgradeWoodCost          * 0.5f) + (int)(buildingData.level3UpgradeWoodCost          * 0.5f),
                        (int)(buildingData.stoneCost         * 0.5f) + (int)(buildingData.level2UpgradeStoneCost         * 0.5f) + (int)(buildingData.level3UpgradeStoneCost         * 0.5f),
                        (int)(buildingData.ironOreCost       * 0.5f) + (int)(buildingData.level2UpgradeIronOreCost       * 0.5f) + (int)(buildingData.level3UpgradeIronOreCost       * 0.5f),
                        (int)(buildingData.ironBarCost       * 0.5f) + (int)(buildingData.level2UpgradeIronBarCost       * 0.5f) + (int)(buildingData.level3UpgradeIronBarCost       * 0.5f)
                    );
                    break;
                default:
                    Debug.LogError("Something went wrong trying to sell");
                    break;
            }
        }
        else
        {
            switch (level)
            {
                case 1:
                    sellResourceData = new
                    (
                        buildingData.spiritEssenceCost,
                        buildingData.woodCost,
                        buildingData.stoneCost,
                        buildingData.ironOreCost,
                        buildingData.ironBarCost
                    );
                    break;
                case 2:
                    sellResourceData = new
                    (
                        buildingData.spiritEssenceCost + buildingData.level2UpgradeSpiritEssenceCost,
                        buildingData.woodCost          + buildingData.level2UpgradeWoodCost,
                        buildingData.stoneCost         + buildingData.level2UpgradeStoneCost,
                        buildingData.ironOreCost       + buildingData.level2UpgradeIronOreCost,
                        buildingData.ironBarCost       + buildingData.level2UpgradeIronBarCost
                    );
                    break;
                case 3:
                    sellResourceData = new
                    (
                        buildingData.spiritEssenceCost + buildingData.level2UpgradeSpiritEssenceCost + buildingData.level3UpgradeSpiritEssenceCost,
                        buildingData.woodCost          + buildingData.level2UpgradeWoodCost          + buildingData.level3UpgradeWoodCost,
                        buildingData.stoneCost         + buildingData.level2UpgradeStoneCost         + buildingData.level3UpgradeStoneCost,
                        buildingData.ironOreCost       + buildingData.level2UpgradeIronOreCost     + buildingData.level3UpgradeIronOreCost,
                        buildingData.ironBarCost       + buildingData.level2UpgradeIronBarCost       + buildingData.level3UpgradeIronBarCost
                    );
                    break;
                default:
                    Debug.LogError("Something went wrong trying to sell");
                    break;
            }
        }

        return sellResourceData;
    }

}
