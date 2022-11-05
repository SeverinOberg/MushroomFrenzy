using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour, IPurchaseable
{
    [SerializeField] private ItemSO itemSO;

    public virtual void Purchase(PlayerController player, int quantity) {}

    public virtual bool Validate(PlayerController player, int quantity) { return true; }
}
