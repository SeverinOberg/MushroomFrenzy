using UnityEngine;

public class Building : Unit
{
    [SerializeField] private PlayerController owner;
    // Do not set any of the data in this Scriptable Object, only get.
    public BuildingSO buildingData;

    private int level;
    private int maxLevel;

    public int Level    { get { return level; }    private set { level    = value; } }
    public int MaxLevel { get { return maxLevel; } private set { maxLevel = value; } }

    protected override void Awake()
    {
        base.Awake();
        Level = 1;
        MaxLevel = buildingData.maxLevel;
    }

    public void SetOwner(PlayerController owner)
    {
        if (!this.owner)
        {
            this.owner = owner;
        }
    }

    public PlayerController GetOwner()
    {
        return owner;
    }

    public bool UpgradeBuilding()
    {
        if (!IsOwnerWithinInteractRange())
        {
            return false;
        }

        if (MaxLevel == 0 || Level >= MaxLevel)
        {
            UIGame.LogToScreen("Building is already at max level");
            return false;
        }

        if (!owner.resourceManager.PayForUpgrade(buildingData, Level))
        {
            return false;
        }

        Level++;

        switch (Level)
        {
            case 2:
                MaxHealth += 15;
                Health += 15;
                spriteRenderer.color = new Color(0.85f, 0.85f, 0.85f);
                break;
            case 3:
                MaxHealth += 15;
                Health += 15;
                spriteRenderer.color = new Color(0.70f, 0.70f, 0.70f);
                break;
            default:
                Debug.LogError("Something went wrong trying to upgrade building, unknown level");
                return false;
        }

        return true;
    }

    public bool SellBuilding()
    {
        if (!IsOwnerWithinInteractRange())
        {
            return false;
        }

        ResourceObject sellResourceData;
        if (Health < MaxHealth)
        {
            sellResourceData = owner.resourceManager.GetSellDataByLevel(buildingData, Level, damaged: true);
        }
        else
        {
            sellResourceData = owner.resourceManager.GetSellDataByLevel(buildingData, Level, damaged: false);
        }
        owner.resourceManager.SellBuilding(sellResourceData);

        Die(0.3f);
        return true;
    }

    public bool RepairBuilding()
    {
        if (!IsOwnerWithinInteractRange())
        {
            return false;
        }

        if (!owner.resourceManager.HasSufficientResourcesToRepair(buildingData, level, out ResourceObject repairCostData))
        {
            return false;
        }

        // If the building has taken damage, the resources returned will be halfed
        if (Health >= MaxHealth)
        {
            UIGame.LogToScreen($"Already fully repaired");
            return false;
        }

        
        if (!owner.resourceManager.PayForRepair(repairCostData))
        {
            return false;
        }
        
        // Repairing always heals half the buildings max health
        Heal(MaxHealth * 0.5f);

        return true;
    }

    private bool IsOwnerWithinInteractRange()
    {
        if (Utilities.GetDistanceBetween(owner.transform.position, transform.position) >= owner.InteractRange)
        {
            UIGame.LogToScreen($"Too far away");
            return false;
        }

        return true;
    }

}