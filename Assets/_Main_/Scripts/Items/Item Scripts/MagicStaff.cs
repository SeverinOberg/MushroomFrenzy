using UnityEngine;

public class MagicStaff : Item
{
    public override void Purchase(PlayerController player, int quantity)
    {
        Debug.Log("Hello from MagicStaff");
    }
}
