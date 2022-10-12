using UnityEngine;

public class Building : Unit
{
    // Do not set any of the data in this Scriptable Object, only get.
    public BuildingSO buildingData;

    private int level;
    private int maxLevel;

    public int Level { get { return level; } private set { level = value; OnSetLevel?.Invoke(); } }
    public int MaxLevel { get { return maxLevel; } private set { maxLevel = value; } }

    public System.Action OnSetLevel;

    private LayerMask playerLayerMask;
    private float scanForPlayerInteractRadius = 5f;

    protected override void Start()
    {
        base.Start();
        Level = 1;
        MaxLevel = buildingData.maxLevel;

        playerLayerMask = LayerMask.GetMask("Player");
    }

    public bool UpgradeBuilding()
    {
        if (!IsPlayerWithinInteractRange())
        {
            return false;
        }

        if (MaxLevel == 0 || Level >= MaxLevel)
        {
            UIGame.LogToScreen("Building is already at max level");
            return false;
        }

        if (!ResourceManager.Instance.PayForUpgrade(buildingData, Level))
        {
            return false;
        }

        switch (Level)
        {
            case 1:
                MaxHealth += 15;
                Health += 15;
                break;
            case 2:
                MaxHealth += 15;
                Health += 15;
                break;
            case 3:
                MaxHealth += 15;
                Health += 15;
                break;
            default:
                Debug.LogError("Something went wrong trying to upgrade building, unknown level");
                return false;
        }

        Level++;
        return true;
    }

    public bool SellBuilding()
    {
        if (!IsPlayerWithinInteractRange())
        {
            return false;
        }

        ResourceDataObject sellResourceData;
        if (Health < MaxHealth)
        {
            sellResourceData = ResourceManager.Instance.GetSellDataByLevel(buildingData, Level, damaged: true);
        }
        else
        {
            sellResourceData = ResourceManager.Instance.GetSellDataByLevel(buildingData, Level, damaged: false);
        }
        ResourceManager.Instance.Sell(sellResourceData);

        Die(0.3f);
        return true;
    }

    public bool RepairBuilding()
    {
        if (!IsPlayerWithinInteractRange())
        {
            return false;
        }

        if (!ResourceManager.Instance.HasSufficientResourcesToRepair(buildingData, level, out ResourceDataObject repairCostData))
        {
            return false;
        }

        // If the building has taken damage, the resources returned will be halfed
        if (Health >= MaxHealth)
        {
            UIGame.LogToScreen($"Already fully repaired");
            return false;
        }

        
        if (!ResourceManager.Instance.PayForRepair(repairCostData))
        {
            return false;
        }
        
        // Repairing always heals half the buildings max health
        Heal(MaxHealth * 0.5f);
        return true;
    }

    private bool IsPlayerWithinInteractRange()
    {
        if (!Physics2D.OverlapCircle(transform.position, scanForPlayerInteractRadius, playerLayerMask))
        {
            UIGame.LogToScreen($"Too far away");
            return false;
        }

        return true;
    }

}