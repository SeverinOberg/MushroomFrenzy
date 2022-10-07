using UnityEngine;

public class ResourceManager : MonoBehaviour 
{

    public static ResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public enum PaymentType
    {
        Build,
        Upgrade,
    }

    public static System.Action<int> OnWoodChanged;
    public static System.Action<int> OnStoneChanged;
    public static System.Action<int> OnMetalChanged;

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
                Wood  -= buildingData.level1UpgradeWoodCost;
                Stone -= buildingData.level1UpgradeStoneCost;
                Metal -= buildingData.level1UpgradeMetalCost;
                break;
            case 2:
                Wood  -= buildingData.level2UpgradeWoodCost;
                Stone -= buildingData.level2UpgradeStoneCost;
                Metal -= buildingData.level2UpgradeMetalCost;
                break;
            case 3:
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
                if (Wood  >= buildingData.level1UpgradeWoodCost &&
                    Stone >= buildingData.level1UpgradeStoneCost &&
                    Metal >= buildingData.level1UpgradeMetalCost)
                    return true;
                    break;
            case 2:
                if (Wood  >= buildingData.level2UpgradeWoodCost &&
                    Stone >= buildingData.level2UpgradeStoneCost &&
                    Metal >= buildingData.level2UpgradeMetalCost)
                    return true;
                break;
            case 3:
                if (Wood  >= buildingData.level3UpgradeWoodCost &&
                    Stone >= buildingData.level3UpgradeStoneCost &&
                    Metal >= buildingData.level3UpgradeMetalCost)
                    return true;
                break;
            default:
                Debug.LogError("Checking for sufficient resources to upgrade failed, unknown level");
                break;
        }

        UIGame.LogToScreen($"Not enough resources to upgrade");
        return false;
    }

}
