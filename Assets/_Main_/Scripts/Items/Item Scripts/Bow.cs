using UnityEngine;

public class Bow : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        Debug.Log("Hello from Bow");
    }
}
