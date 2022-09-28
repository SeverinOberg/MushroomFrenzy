using System.Collections;
using UnityEngine;

public class BuildingDrag : MonoBehaviour 
{

    #region Variables & Properties

    private Unit unit;
    private SpriteRenderer highlightSprite;
    private BoxCollider2D boxCollider;

    private Vector2 mousePosition;

    private bool buildMode;

    private float dragSpeed = 15;

    #endregion

    #region Unity

    private void Awake()
    {
        unit = GetComponent<Unit>();
        highlightSprite = GetComponentsInChildren<SpriteRenderer>()[0];
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        BuildingSystem.OnBuildMode += SetBuildMode;
    }

    private void OnDisable()
    {
        BuildingSystem.OnBuildMode -= SetBuildMode;
    }

    private void LateUpdate()
    {

        if (buildMode)
        {
            mousePosition = Utilities.GetMouseWorldPosition();
            transform.position = Vector3.Lerp(transform.position, BuildingSystem.Instance.SnapCoordinateToGrid(mousePosition), Time.deltaTime * dragSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape) && buildMode)
        {
            BuildingSystem.OnBuildMode?.Invoke(false);
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && buildMode)
        {
            transform.position = BuildingSystem.Instance.SnapCoordinateToGrid(mousePosition);

            Collider2D overlappingCollider = Physics2D.OverlapBox(transform.position, transform.localScale * 0.97f, 0.0f);
            if (overlappingCollider)
            {
                unit.BlinkRed();
                return;
            }

            if (TryGetComponent(out Building building))
            {
                ResourceManager.Instance.Wood -= building.buildingData.woodCost;
                ResourceManager.Instance.Stone -= building.buildingData.stoneCost;
            }
            else
            {
                Debug.LogError("Could not find 'Building' component, this is unexpected");
                return;
            }

            if (TryGetComponent(out Turret turret))
            {
                turret.isSleeping = false;
            }

            BuildingSystem.OnBuildMode?.Invoke(false);
            Destroy(this);
        }

    }

    #endregion

    #region Methods

    public void SetBuildMode(bool value)
    {
        buildMode = value;
        if (buildMode)
        {
            boxCollider.enabled = false;
        }
        else
        {
            boxCollider.enabled = true;
        }
    }

    #endregion

}
