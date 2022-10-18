using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour 
{

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
        SpiritEssence = 100;
        Wood          = 100;
        Stone         = 100;
        IronOre       = 100;
        IronBar       = 100;
    }

    public bool PayForBuild(BuildingSO buildingData)
    {
        if (!HasSufficientResourcesToBuild(buildingData))
        {
            return false;
        }

        DecreaseResources(buildingData);
      
        return true;
    }

    public bool PayForUpgrade(BuildingSO buildingData, int level)
    {
        if (!HasSufficientResourcesToUpgrade(buildingData, level))
        {
            return false;
        }

        DecreaseResources(buildingData, level);

        return true;
    }

    public bool PayForRepair(ResourceObject resourceData)
    {
        DecreaseResources(resourceData.wood, resourceData.stone, resourceData.ironBar);

        return true;
    }

    public void SellBuilding(ResourceObject resourceData)
    {
        IncreaseResources(resourceData.wood, resourceData.stone, resourceData.ironBar);
    }

    private void IncreaseResources(int wood, int stone, int ironBar)
    {
        Wood    += wood;
        Stone   += stone;
        IronBar += ironBar;
    }

    private void DecreaseResources(int wood, int stone, int ironBar)
    {
        Wood    -= wood;
        Stone   -= stone;
        IronBar -= ironBar;
    }

    private void DecreaseResources(BuildingSO buildingData)
    {
        Wood    -= buildingData.woodCost;
        Stone   -= buildingData.stoneCost;
        IronBar -= buildingData.ironBarCost;
    }

    private void DecreaseResources(BuildingSO buildingData, int level)
    {
        switch (level)
        {
            case 1:
                Wood  -= buildingData.level2UpgradeWoodCost;
                Stone -= buildingData.level2UpgradeStoneCost;
                IronBar -= buildingData.level2UpgradeIronBarCost;
                break;
            case 2:
                Wood  -= buildingData.level3UpgradeWoodCost;
                Stone -= buildingData.level3UpgradeStoneCost;
                IronBar -= buildingData.level3UpgradeIronBarCost;
                break;
            default:
                Debug.LogError("Decreasing resources failed, unknown level");
                break;
        }
    }

    public bool HasSufficientResourcesToBuild(BuildingSO buildingData)
    {
        if (Wood >= buildingData.woodCost && Stone >= buildingData.stoneCost && IronBar >= buildingData.ironBarCost)
            return true;
        
        UIGame.LogToScreen($"Not enough resources to build");
        return false;
    }

    public bool HasSufficientResourcesToUpgrade(BuildingSO buildingData, int level)
    {
        switch (level)
        {
            case 1:
                if (Wood  >= buildingData.level2UpgradeWoodCost &&
                    Stone >= buildingData.level2UpgradeStoneCost &&
                    IronBar >= buildingData.level2UpgradeIronBarCost)
                    return true;
                break;
            case 2:
                if (Wood  >= buildingData.level3UpgradeWoodCost &&
                    Stone >= buildingData.level3UpgradeStoneCost &&
                    IronBar >= buildingData.level3UpgradeIronBarCost)
                    return true;
                break;
            case 3:
                // Do nothing, this is the max level.
                return true;
            default:
                Debug.LogError("Checking for sufficient resources to upgrade failed, unknown level");
                break;
        }

        UIGame.LogToScreen($"Not enough resources to upgrade");
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
                if (Wood >= repairCostData.wood && Stone >= repairCostData.stone && IronBar >= repairCostData.ironBar)
                {
                    return true;
                }
                break;
            case 2:
                repairCostData = GetRepairDataByLevel(buildingData, 2);
                if (Wood >= repairCostData.wood && Stone >= repairCostData.stone && IronBar >= repairCostData.ironBar)
                {
                    return true;
                }
                break;
            case 3:
                repairCostData = GetRepairDataByLevel(buildingData, 3);
                if (Wood >= repairCostData.wood && Stone >= repairCostData.stone && IronBar >= repairCostData.ironBar)
                {
                    return true;
                }
                break;
            default:
                Debug.LogError("Checking for sufficient resources to repair failed, unknown level");
                break;
        }

        UIGame.LogToScreen($"Not enough resources to repair");
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
                    (int)Math.Round(buildingData.woodCost    * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.stoneCost   * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.ironBarCost * 0.5f, MidpointRounding.AwayFromZero)
                );
                break;
            case 2:
                result = new
                (
                    (int)Math.Round(buildingData.level2UpgradeWoodCost    * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level2UpgradeStoneCost   * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level2UpgradeIronBarCost * 0.5f, MidpointRounding.AwayFromZero)
                );
                break;
            case 3:
                result = new
                (
                    (int)Math.Round(buildingData.level3UpgradeWoodCost    * 0.75f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level3UpgradeStoneCost   * 0.75f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level3UpgradeIronBarCost * 0.75f, MidpointRounding.AwayFromZero)
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
                        (int)(buildingData.woodCost    * 0.5f),
                        (int)(buildingData.stoneCost   * 0.5f),
                        (int)(buildingData.ironBarCost * 0.5f)
                    );
                    break;
                case 2:
                    sellResourceData = new
                    (
                        (int)(buildingData.woodCost    * 0.5f) + (int)(buildingData.level2UpgradeWoodCost    * 0.5f),
                        (int)(buildingData.stoneCost   * 0.5f) + (int)(buildingData.level2UpgradeStoneCost   * 0.5f),
                        (int)(buildingData.ironBarCost * 0.5f) + (int)(buildingData.level2UpgradeIronBarCost * 0.5f)
                    );
                    break;
                case 3:
                    sellResourceData = new
                    (
                        (int)(buildingData.woodCost    * 0.5f) + (int)(buildingData.level2UpgradeWoodCost    * 0.5f) + (int)(buildingData.level3UpgradeWoodCost    * 0.5f),
                        (int)(buildingData.stoneCost   * 0.5f) + (int)(buildingData.level2UpgradeStoneCost   * 0.5f) + (int)(buildingData.level3UpgradeStoneCost   * 0.5f),
                        (int)(buildingData.ironBarCost * 0.5f) + (int)(buildingData.level2UpgradeIronBarCost * 0.5f) + (int)(buildingData.level3UpgradeIronBarCost * 0.5f)
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
                        buildingData.woodCost,
                        buildingData.stoneCost,
                        buildingData.ironBarCost
                    );
                    break;
                case 2:
                    sellResourceData = new
                    (
                        buildingData.woodCost  + buildingData.level2UpgradeWoodCost,
                        buildingData.stoneCost + buildingData.level2UpgradeStoneCost,
                        buildingData.ironBarCost + buildingData.level2UpgradeIronBarCost
                    );
                    break;
                case 3:
                    sellResourceData = new
                    (
                        buildingData.woodCost  + buildingData.level2UpgradeWoodCost  + buildingData.level3UpgradeWoodCost,
                        buildingData.stoneCost + buildingData.level2UpgradeStoneCost + buildingData.level3UpgradeStoneCost,
                        buildingData.ironBarCost + buildingData.level2UpgradeIronBarCost + buildingData.level3UpgradeIronBarCost
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
