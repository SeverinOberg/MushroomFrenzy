public class ResourceObject
{
    public int wood;
    public int stone;
    public int ironOre;
    public int ironBar;

    public ResourceObject(int wood, int stone, int ironOre, int ironBar)
    {
        this.wood = wood;
        this.stone = stone;
        this.ironOre = ironOre;
        this.ironBar = ironBar;
    }

    public ResourceObject(int wood, int stone, int ironBar)
    {
        this.wood = wood;
        this.stone = stone;
        this.ironBar = ironBar;
    }
}
