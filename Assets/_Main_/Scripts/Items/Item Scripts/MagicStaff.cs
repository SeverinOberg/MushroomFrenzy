using UnityEngine;

public class MagicStaff : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        player.magicStaffUpgrade = true;
        player.equippedWeapon = PlayerController.EquippedWeapon.MagicStaff;
        player.UIManager.EnableArmoryMagicStaffSlot();

        UIManager.LogToScreen("You can change weapons in base");
    }

    public override bool Validate(PlayerController player, int quantity)
    {
        if (player.magicStaffUpgrade)
        {
            UIManager.LogToScreen("You can only wield 1 magic staff");
            return false;
        }

        if (quantity > 1)
        {
            UIManager.LogToScreen("Can only buy 1 magic staff");
            return false;
        }

        return true;
    }
}
