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

    private int wood;
    private int stone;

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

    private void Start()
    {
        Wood = 20;
        Stone = 20;
    }

    public bool HasSufficientResources(BuildingSO building)
    {
        if (Wood >= building.woodCost && Stone >= building.stoneCost)
        {
            return true;
        }
        UIGame.LogToScreen($"Not enough resources");
        return false;
    }

}
