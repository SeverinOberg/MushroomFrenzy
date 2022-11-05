using UnityEngine;

public class Apple : Item
{
    private float healPercentage = 0.25f;

    public override void Purchase(PlayerController player, int quantity)
    {
        player.Heal(player.MaxHealth * healPercentage * quantity);
    }

    public override bool Validate(PlayerController player, int quantity)
    {
        if (player.Health >= player.MaxHealth)
        {
            UIManager.LogToScreen("Already at full health");
            return false;
        }

        return true;
    }
}
