using UnityEngine;

public class Wood : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        ResourceObject payload = new (wood: quantity);
        player.ResourceManager.IncreaseResources(payload);
    }
}
