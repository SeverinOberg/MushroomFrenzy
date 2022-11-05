using UnityEngine;

public class IronBar : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        ResourceObject payload = new (ironBar: quantity);
        player.ResourceManager.IncreaseResources(payload);
    }
}
