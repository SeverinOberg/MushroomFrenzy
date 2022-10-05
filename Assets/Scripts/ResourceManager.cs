using UnityEngine;

public class ResourceManager : MonoBehaviour 
{

    public static ResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static System.Action<int> OnWoodChanged;
    public static System.Action<int> OnStoneChanged;
    public static System.Action<int> OnMetalChanged;

    public GameObject[] resources;

    private int wood;
    private int stone;
    private int metal;

    public int Wood
    {
        get { return wood; }
        set
        {
            wood = value;
            OnWoodChanged?.Invoke(wood);
        }
    }

    public int Stone 
    { 
        get { return stone; } 
        set 
        {
            stone = value;
            OnStoneChanged?.Invoke(stone);
        } 
    }

    public int Metal
    {
        get { return metal; }
        set
        {
            metal = value;
            OnMetalChanged?.Invoke(metal);
        }
    }

    private void Start()
    {
        Wood = 20;
        Stone = 20;
        Metal = 20;
    }

    public bool HasSufficientResources(BuildingSO building)
    {
        if (Wood >= building.woodCost && Stone >= building.stoneCost && Metal >= building.metalCost)
        {
            return true;
        }
        UIGame.LogToScreen($"Not enough resources");
        return false;
    }

}
