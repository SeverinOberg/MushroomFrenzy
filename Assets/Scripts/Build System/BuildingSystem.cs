using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour 
{
    #region Propertes

    public static BuildingSystem current;

    public GridLayout gridLayout;
    private Grid grid;

    [SerializeField] private Tilemap mainTilemap;
    [SerializeField] private TileBase whiteTile;

    private PlaceableObject objectToPlace;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        current = this;
        grid = gridLayout.GetComponent<Grid>();
    }

    private void OnEnable()
    {
        BuildButton.OnBuildTurret += InitializeWithObject;
    }

    private void OnDisable()
    {
        BuildButton.OnBuildTurret -= InitializeWithObject;
    }

    #endregion

    #region Building Placement

    public void InitializeWithObject(GameObject prefab)
    {
        Vector3 position = SnapCoordinateToGrid(Utilities.GetMouseWorldPosition());

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();

        var objDrag = obj.AddComponent<ObjectDrag>();

        objDrag.turret = obj.GetComponent<Turret>();
        objDrag.SetBuildMode(true);
    }

    #endregion

    #region Utilities

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPosition);
        return position;
    }

    #endregion

}
