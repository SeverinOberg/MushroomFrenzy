public class PopupData
{
    public int    amount;
    public string resourceName;
    public bool   isIncrease;

    public PopupData(int amount, string resourceName, bool isIncrease)
    {
        this.amount = amount;
        this.resourceName = resourceName;
        this.isIncrease = isIncrease;
    }
}