using UnityEngine;

public class BuildingSystem : MonoBehaviour 
{
    #region Variables & Properties

    public static BuildingSystem Instance;

    public GridLayout gridLayout;
    private Grid grid;

    private bool buildMode;

    public static System.Action<bool> OnBuildMode;

    #endregion

    #region Unity

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        grid = gridLayout.GetComponent<Grid>();
    }

    private void OnEnable()
    {
        OnBuildMode                 += SetBuildMode;
        BuildButton.OnBuildBuilding += InitializeWithObject;
    }

    private void OnDisable()
    {
        OnBuildMode                 -= SetBuildMode;
        BuildButton.OnBuildBuilding -= InitializeWithObject;
    }

    #endregion

    #region Methods

    public void InitializeWithObject(GameObject prefab)
    {
        if (buildMode)
        {
            UIGame.LogToScreen("Place or cancel your current building to build another");
            return;
        }

        Vector3 position = SnapCoordinateToGrid(Utilities.GetMouseWorldPosition());

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        BuildingDrag buildingDrag = obj.AddComponent<BuildingDrag>();

        OnBuildMode?.Invoke(true);
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPosition);
        return position;
    }

    private void SetBuildMode(bool value)
    {
        buildMode = value;
    }

    #endregion

}
