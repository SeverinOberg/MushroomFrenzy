using UnityEngine;

public class BuildingSystem : MonoBehaviour 
{
    #region Variables & Properties

    public static BuildingSystem Instance;

    public GridLayout gridLayout;
    private Grid grid;

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
        BuildButton.OnBuildBuilding += InitializeWithObject;
    }

    private void OnDisable()
    {
        BuildButton.OnBuildBuilding -= InitializeWithObject;
    }

    #endregion

    #region Methods

    public void InitializeWithObject(GameObject prefab)
    {
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

    #endregion

}
