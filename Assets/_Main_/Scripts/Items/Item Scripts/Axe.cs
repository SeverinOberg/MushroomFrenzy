using UnityEngine;

public class Axe : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        player.axeUpgrade = true;
    }

    public override bool Validate(PlayerController player, int quantity)
    {
        if (player.axeUpgrade)
        {
            UIManager.LogToScreen("You can only wield 1 axe");
            return false;
        }

        if (quantity > 1)
        {
            UIManager.LogToScreen("Can only buy 1 axe");
            return false;
        }

        return true;
    }
}
