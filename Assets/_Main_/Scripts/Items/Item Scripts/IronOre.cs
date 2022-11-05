using UnityEngine;

public class IronOre : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        ResourceObject payload = new (ironOre: quantity);
        player.ResourceManager.IncreaseResources(payload);
    }
}
