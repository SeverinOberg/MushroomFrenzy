using UnityEngine;

public class ResourceManager : MonoBehaviour 
{

    public static ResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private UIGame uiGame;

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
        Wood = 10;
        Stone = 10;
    }

    public bool CanBuild(int woodCost, int stoneCost)
    {
        if (Wood >= woodCost && Stone >= stoneCost)
        {
            Wood -= woodCost;
            Stone -= stoneCost;
            return true;
        }
        return false;
    }
}
