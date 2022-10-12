using System;
using System.Transactions;
using UnityEngine;

public class ResourceDataObject
{
    public int wood;
    public int stone;
    public int metal;

    public ResourceDataObject(int wood, int stone, int metal)
    {
        this.wood = wood;
        this.stone = stone;
        this.metal = metal;
    }
}

public class ResourceManager : MonoBehaviour 
{

    public static ResourceManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public enum PaymentType
    {
        Build,
        Upgrade,
    }

    public static Action<int> OnWoodChanged;
    public static Action<int> OnStoneChanged;
    public static Action<int> OnMetalChanged;

    public GameObject[] resources;

    private int wood;
    private int stone;
    private int metal;

    public int Wood
    {
        get { return wood; }
        set
        {
            wood = value;
            OnWoodChanged?.Invoke(wood);
        }
    }

    public int Stone 
    { 
        get { return stone; } 
        set 
        {
            stone = value;
            OnStoneChanged?.Invoke(stone);
        } 
    }

    public int Metal
    {
        get { return metal; }
        set
        {
            metal = value;
            OnMetalChanged?.Invoke(metal);
        }
    }

    private void Start()
    {
        Wood = 100;
        Stone = 100;
        Metal = 100;
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

    public bool PayForRepair(ResourceDataObject resourceData)
    {
        DecreaseResources(resourceData.wood, resourceData.stone, resourceData.metal);

        return true;
    }

    public void Sell(ResourceDataObject resourceData)
    {
        IncreaseResources(resourceData.wood, resourceData.stone, resourceData.metal);
    }

    private void IncreaseResources(int wood, int stone, int metal)
    {
        Wood  += wood;
        Stone += stone;
        Metal += metal;
    }

    private void DecreaseResources(int wood, int stone, int metal)
    {
        Wood  -= wood;
        Stone -= stone;
        Metal -= metal;
    }

    private void DecreaseResources(BuildingSO buildingData)
    {
        Wood  -= buildingData.woodCost;
        Stone -= buildingData.stoneCost;
        Metal -= buildingData.metalCost;
    }

    private void DecreaseResources(BuildingSO buildingData, int level)
    {
        switch (level)
        {
            case 1:
                Wood  -= buildingData.level2UpgradeWoodCost;
                Stone -= buildingData.level2UpgradeStoneCost;
                Metal -= buildingData.level2UpgradeMetalCost;
                break;
            case 2:
                Wood  -= buildingData.level3UpgradeWoodCost;
                Stone -= buildingData.level3UpgradeStoneCost;
                Metal -= buildingData.level3UpgradeMetalCost;
                break;
            default:
                Debug.LogError("Decreasing resources failed, unknown level");
                break;
        }
    }

    public bool HasSufficientResourcesToBuild(BuildingSO buildingData)
    {
        if (Wood  >= buildingData.woodCost  &&
            Stone >= buildingData.stoneCost &&
            Metal >= buildingData.metalCost)
        {
            return true;
        }

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
                    Metal >= buildingData.level2UpgradeMetalCost)
                    return true;
                break;
            case 2:
                if (Wood  >= buildingData.level3UpgradeWoodCost &&
                    Stone >= buildingData.level3UpgradeStoneCost &&
                    Metal >= buildingData.level3UpgradeMetalCost)
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
    public bool HasSufficientResourcesToRepair(BuildingSO buildingData, int level, out ResourceDataObject repairCostData)
    {
        repairCostData = null;

        switch (level)
        {
            case 1:
                repairCostData = GetRepairDataByLevel(buildingData, 1);
                if (Wood >= repairCostData.wood && Stone >= repairCostData.stone && Metal >= repairCostData.metal)
                {
                    return true;
                }
                break;
            case 2:
                repairCostData = GetRepairDataByLevel(buildingData, 2);
                if (Wood >= repairCostData.wood && Stone >= repairCostData.stone && Metal >= repairCostData.metal)
                {
                    return true;
                }
                break;
            case 3:
                repairCostData = GetRepairDataByLevel(buildingData, 3);
                if (Wood >= repairCostData.wood && Stone >= repairCostData.stone && Metal >= repairCostData.metal)
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

    public ResourceDataObject GetRepairDataByLevel(BuildingSO buildingData, int level)
    {
        ResourceDataObject result = null;

        switch (level)
        {

            case 1:
                result = new
                (
                    (int)Math.Round(buildingData.woodCost  * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.stoneCost * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.metalCost * 0.5f, MidpointRounding.AwayFromZero)
                );
                break;
            case 2:
                result = new
                (
                    (int)Math.Round(buildingData.level2UpgradeWoodCost  * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level2UpgradeStoneCost * 0.5f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level2UpgradeMetalCost * 0.5f, MidpointRounding.AwayFromZero)
                );
                break;
            case 3:
                result = new
                (
                    (int)Math.Round(buildingData.level3UpgradeWoodCost  * 0.75f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level3UpgradeStoneCost * 0.75f, MidpointRounding.AwayFromZero),
                    (int)Math.Round(buildingData.level3UpgradeMetalCost * 0.75f, MidpointRounding.AwayFromZero)
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
    public ResourceDataObject GetSellDataByLevel(BuildingSO buildingData, int level, bool damaged)
    {
        ResourceDataObject sellResourceData = null;

        if (damaged)
        {
            switch (level)
            {
                case 1:
                    sellResourceData = new
                    (
                        (int)(buildingData.woodCost * 0.5f),
                        (int)(buildingData.stoneCost * 0.5f),
                        (int)(buildingData.metalCost * 0.5f)
                    );
                    break;
                case 2:
                    sellResourceData = new
                    (
                        (int)(buildingData.woodCost  * 0.5f) + (int)(buildingData.level2UpgradeWoodCost * 0.5f),
                        (int)(buildingData.stoneCost * 0.5f) + (int)(buildingData.level2UpgradeStoneCost * 0.5f),
                        (int)(buildingData.metalCost * 0.5f) + (int)(buildingData.level2UpgradeMetalCost * 0.5f)
                    );
                    break;
                case 3:
                    sellResourceData = new
                    (
                        (int)(buildingData.woodCost  * 0.5f) + (int)(buildingData.level2UpgradeWoodCost  * 0.5f) + (int)(buildingData.level3UpgradeWoodCost  * 0.5f),
                        (int)(buildingData.stoneCost * 0.5f) + (int)(buildingData.level2UpgradeStoneCost * 0.5f) + (int)(buildingData.level3UpgradeStoneCost * 0.5f),
                        (int)(buildingData.metalCost * 0.5f) + (int)(buildingData.level2UpgradeMetalCost * 0.5f) + (int)(buildingData.level3UpgradeMetalCost * 0.5f)
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
                        buildingData.metalCost
                    );
                    break;
                case 2:
                    sellResourceData = new
                    (
                        buildingData.woodCost  + buildingData.level2UpgradeWoodCost,
                        buildingData.stoneCost + buildingData.level2UpgradeStoneCost,
                        buildingData.metalCost + buildingData.level2UpgradeMetalCost
                    );
                    break;
                case 3:
                    sellResourceData = new
                    (
                        buildingData.woodCost  + buildingData.level2UpgradeWoodCost  + buildingData.level3UpgradeWoodCost,
                        buildingData.stoneCost + buildingData.level2UpgradeStoneCost + buildingData.level3UpgradeStoneCost,
                        buildingData.metalCost + buildingData.level2UpgradeMetalCost + buildingData.level3UpgradeMetalCost
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
