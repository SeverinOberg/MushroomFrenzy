using UnityEngine;

public class Bow : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        player.bowUpgrade = true;
        player.equippedWeapon = PlayerController.EquippedWeapon.Bow;
        player.UIManager.EnableArmoryBowSlot();

        UIManager.LogToScreen("You can change weapons in base");
    }

    public override bool Validate(PlayerController player, int quantity)
    {
        if (player.bowUpgrade)
        {
            UIManager.LogToScreen("You can only wield 1 bow");
            return false;
        }

        if (quantity > 1)
        {
            UIManager.LogToScreen("Can only buy 1 bow");
            return false;
        }

        return true;
    }
}
