using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSystem : MonoBehaviour 
{
    #region Variables & Properties

    public static BuildingSystem Instance;

    [SerializeField] private GridLayout gridLayout;
    private Grid grid;

    [HideInInspector] public bool buildMode;

    #endregion

    #region Unity

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        

        grid = gridLayout.GetComponent<Grid>();
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

        obj.AddComponent<BuildingDrag>();
        buildMode = true;
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPosition);
        return position;
    }

    #endregion
}
