public interface IPurchaseable 
{
    public abstract void Purchase(PlayerController player, int quantity);
    public abstract bool Validate(PlayerController player, int quantity);
}
