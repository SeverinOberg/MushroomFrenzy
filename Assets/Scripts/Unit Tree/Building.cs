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

    public void UpgradeBuilding()
    {
        if (!IsPlayerWithinInteractRange())
        {
            return;
        }

        if (MaxLevel == 0 || Level >= MaxLevel)
        {
            UIGame.LogToScreen("Building is already at max level");
            return;
        }

        if (!ResourceManager.Instance.PayForUpgrade(buildingData, Level))
        {
            return;
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
                return;
        }

        Level++;
    }

    public void SellBuilding()
    {
        if (!IsPlayerWithinInteractRange())
        {
            return;
        }

        // If the building has taken damage, the resources returned will be halfed
        if (Health < MaxHealth)
        {
            ResourceManager.Instance.Wood += (int)(buildingData.woodCost * 0.5f);
            ResourceManager.Instance.Stone += (int)(buildingData.stoneCost * 0.5f);
            ResourceManager.Instance.Metal += (int)(buildingData.metalCost * 0.5f);
        }
        else
        {
            ResourceManager.Instance.Wood += buildingData.woodCost;
            ResourceManager.Instance.Stone += buildingData.stoneCost;
            ResourceManager.Instance.Metal += buildingData.metalCost;
        }

        Destroy(gameObject);
    }

    public void RepairBuilding()
    {
        if (!IsPlayerWithinInteractRange())
        {
            return;
        }

        if (!ResourceManager.Instance.HasSufficientResourcesToBuild(buildingData))
        {
            return;
        }

        // If the building has taken damage, the resources returned will be halfed
        if (Health >= MaxHealth)
        {
            UIGame.LogToScreen($"Already fully repaired");
            return;
        }

        // Repairing always costs at least 1 or half (rounded away from zero) the price to build it
        ResourceManager.Instance.Wood -= (int)System.Math.Round((decimal)buildingData.woodCost / 2, System.MidpointRounding.AwayFromZero);
        ResourceManager.Instance.Stone -= (int)System.Math.Round((decimal)buildingData.stoneCost / 2, System.MidpointRounding.AwayFromZero);

        // Repairing always heals half the buildings max health
        Heal(MaxHealth * 0.5f);
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