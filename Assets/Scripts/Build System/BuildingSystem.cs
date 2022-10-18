using UnityEngine;

public class BuildingSystem : MonoBehaviour 
{

    public BuildingTooltip buildingTooltip;

    private GridLayout gridLayout;
    private Grid grid;
    private PlayerController player;

    private bool buildMode;
    public  bool BuildMode { get { return buildMode; } set { buildMode = value; } }

    private void Awake()
    {
        gridLayout = GameObject.Find("Tilemap").GetComponent<GridLayout>();
        grid       = gridLayout.GetComponent<Grid>();

        player     = GetComponent<PlayerController>();
    }

    public void InitializeWithObject(GameObject prefab)
    {
        if (BuildMode)
        {
            UIGame.LogToScreen("Place or cancel your current building to build another");
            return;
        }

        Vector3 position = SnapCoordinateToGrid(Utilities.GetMouseWorldPosition());

        Building building = Instantiate(prefab, position, Quaternion.identity).GetComponent<Building>();
        building.SetOwner(player);
        building.gameObject.AddComponent<BuildingDrag>();
        BuildMode = true;
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPosition);
        return position;
    }

}
