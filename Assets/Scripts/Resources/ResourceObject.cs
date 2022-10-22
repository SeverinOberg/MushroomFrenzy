public class ResourceObject
{
    public int spiritEssence;
    public int wood;
    public int stone;
    public int ironOre;
    public int ironBar;

    public ResourceObject(int spiritEssence, int wood, int stone, int ironOre, int ironBar)
    {
        this.wood = spiritEssence;
        this.wood = wood;
        this.stone = stone;
        this.ironOre = ironOre;
        this.ironBar = ironBar;
    }

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
