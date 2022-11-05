[System.Serializable]
public class ResourceObject
{
    public int spiritEssence;
    public int wood;
    public int stone;
    public int ironOre;
    public int ironBar;

    public ResourceObject(int spiritEssence = 0, int wood = 0, int stone = 0, int ironOre = 0, int ironBar = 0)
    {
        this.spiritEssence = spiritEssence;
        this.wood          = wood;
        this.stone         = stone;
        this.ironOre       = ironOre;
        this.ironBar       = ironBar;
    }

}
