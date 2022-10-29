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
        if (player.resourceManager.HasSufficientResourcesToBuild(building.buildingData, out _))
        {
            player.buildingSystem.InitializeWithObject(building.unitData.prefab);
        }
    }

    public void OnMouseEnter()
    {
        player.buildingSystem.buildingTooltip.OnTooltip?.Invoke
        (
            building.unitData.title, 
            building.unitData.description,
            $"Wood: {building.buildingData.woodCost} Stone: {building.buildingData.stoneCost} Iron Bars: {building.buildingData.ironBarCost}"
        );
    }

    public void OnMouseExit()
    {
        player.buildingSystem.buildingTooltip.OnTooltip?.Invoke("", "", "");
    }

    #endregion
}
