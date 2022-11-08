using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{

    [SerializeField] private GameObject buildingPrefab;
    private PlayerController player;
    private Building building;

    private void Awake()
    {
        building = buildingPrefab.GetComponent<Building>();
        player = GetComponentInParent<PlayerController>();
    }

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
            BuildResourceTooltip()
        );
    }

    public void OnMouseExit()
    {
        player.BuildingSystem.buildingTooltip.OnTooltip?.Invoke("", "", "");
    }

    private string BuildResourceTooltip()
    {
        string resourceTooltip = "";
        if (building.buildingSO.spiritEssenceCost > 0)
        {
            resourceTooltip += $"Spirit Essence: {building.buildingSO.spiritEssenceCost} ";
        }
        if (building.buildingSO.woodCost > 0)
        {
            resourceTooltip += $"Wood: {building.buildingSO.woodCost} ";
        }
        if (building.buildingSO.stoneCost > 0)
        {
            resourceTooltip += $"Stone: {building.buildingSO.stoneCost} ";
        }
        if (building.buildingSO.ironOreCost > 0)
        {
            resourceTooltip += $"Iron Ore: {building.buildingSO.ironOreCost} ";
        }
        if (building.buildingSO.ironBarCost > 0)
        {
            resourceTooltip += $"Iron Bars: {building.buildingSO.ironBarCost} ";
        }
        return resourceTooltip;
    }

}
