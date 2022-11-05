using UnityEngine;

public class Pickaxe : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        player.pickaxeUpgrade = true;
    }

    public override bool Validate(PlayerController player, int quantity)
    {
        if (player.pickaxeUpgrade)
        {
            UIManager.LogToScreen("You can only wield 1 pickaxe");
            return false;
        }

        if (quantity > 1)
        {
            UIManager.LogToScreen("Can only buy 1 pickaxe");
            return false;
        }

        return true;
    }
}
