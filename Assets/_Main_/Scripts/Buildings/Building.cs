using UnityEngine;

public class Building : Unit
{

    [Header("Building")]
    public BuildingSO buildingSO;
    [SerializeField] protected PlayerController owner;
    [SerializeField] private Vector2 selectionSize = new (2, 2);

    private int level;
    private int maxLevel;

    public Vector2 SelectionSize  { get { return selectionSize; } } 
    public int Level              { get { return level;         }   private set { level    = value; } }
    public int MaxLevel           { get { return maxLevel;      }   private set { maxLevel = value; } }

    protected override void Start()
    {
        base.Start();
        Level = 1;
        MaxLevel = buildingSO.maxLevel;
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
            UIManager.LogToScreen("Building is already at max level");
            return false;
        }

        if (!owner.ResourceManager.HasSufficientResourcesToUpgrade(buildingSO, level, out ResourceObject resourceObject))
        {
            return false;
        }

        owner.ResourceManager.PayForUpgrade(resourceObject);

        Level++;

        int healthUpgradeAmount = 15;

        switch (Level)
        {
            case 2:
                AddMaxHealth(healthUpgradeAmount);
                SpriteRenderer.color = new Color(0.85f, 0.85f, 0.85f);
                break;
            case 3:
                AddMaxHealth(healthUpgradeAmount);
                SpriteRenderer.color = new Color(0.70f, 0.70f, 0.70f);
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
            sellResourceData = owner.ResourceManager.GetSellDataByLevel(buildingSO, Level, damaged: true);
        }
        else
        {
            sellResourceData = owner.ResourceManager.GetSellDataByLevel(buildingSO, Level, damaged: false);
        }

        owner.ResourceManager.SellBuilding(sellResourceData);

        SelectionController.OnClearSelection?.Invoke();
        Destroy(gameObject);
        return true;
    }

    public bool RepairBuilding()
    {
        if (!IsOwnerWithinInteractRange())
        {
            return false;
        }

        if (!owner.ResourceManager.HasSufficientResourcesToRepair(buildingSO, level, out ResourceObject repairCostData))
        {
            return false;
        }

        // If the building has taken damage, the resources returned will be halfed
        if (Health >= MaxHealth)
        {
            UIManager.LogToScreen($"Already fully repaired");
            return false;
        }

        owner.ResourceManager.PayForRepair(repairCostData);

        // Repairing always heals half the buildings max health
        Heal(MaxHealth * 0.5f);

        return true;
    }

    private bool IsOwnerWithinInteractRange()
    {
        if (Utilities.GetDistanceBetween(owner.transform.position, transform.position) >= owner.InteractRange)
        {
            UIManager.LogToScreen($"Too far away");
            return false;
        }

        return true;
    }

}