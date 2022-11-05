using UnityEngine;

public class Stone : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        ResourceObject payload = new (stone: quantity);
        player.ResourceManager.IncreaseResources(payload);
    }
}
