using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{

    [SerializeField] private GameObject buildingPrefab;
    private Building building;
    private PlayerController player;
    
    private Button button;

    #region Unity

    private void Awake()
    {
        button = GetComponent<Button>();
        building = buildingPrefab.GetComponent<Building>();
        player = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(BuildButtonClicked);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    #endregion

    #region Methods

    public void BuildButtonClicked()
    {
        if (player.ResourceManager.HasSufficientResourcesToBuild(building.buildingSO, out _))
        {
            player.BuildingSystem.InitializeWithObject(building.Prefab);
        }
    }

    public void OnMouseEnter()
    {
        player.BuildingSystem.buildingTooltip.OnTooltip?.Invoke
        (
            building.Title, 
            building.Description,
            $"Wood: {building.buildingSO.woodCost} Stone: {building.buildingSO.stoneCost} Iron Bars: {building.buildingSO.ironBarCost}"
        );
    }

    public void OnMouseExit()
    {
        player.BuildingSystem.buildingTooltip.OnTooltip?.Invoke("", "", "");
    }

    #endregion
}
