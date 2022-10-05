using UnityEngine;

public class Building : Unit 
{
    // Do not set any of the data in this Scriptable Object, only get.
    public BuildingSO buildingData;

    private int level = 1;
    private int maxLevel;

    protected override void Start()
    {
        base.Start();
        maxLevel = buildingData.maxLevel;
    }

    public void Upgrade()
    {
        if (maxLevel == 0 || level > maxLevel)
        {
            UIGame.LogToScreen("Building is already at max level");
            return;
        }

        if (!ResourceManager.Instance.PayForUpgrade(buildingData, level))
        {
            return;
        }

        UpgradeBuilding();
        level++;
    }

    private void UpgradeBuilding()
    {
        switch (level)
        {
            case 1:
                maxHealth += 15;
                Heal(15);
                break;
            case 2:
                maxHealth += 15;
                Heal(15);
                break;
            case 3:
                maxHealth += 15;
                Heal(15);
                break;
            default:
                Debug.LogError("Something went wrong trying to upgrade building, unknown level");
                return;
        }
    }

}
