using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDrag : MonoBehaviour 
{

    #region Variables & Properties

    private Unit unit;
    private BoxCollider2D boxCollider;

    private Vector2 mousePosition;

    private bool buildMode;

    private float dragSpeed = 15;

    #endregion

    #region Unity

    private void Awake()
    {
        unit = GetComponent<Unit>();
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

        // Drag
        if (buildMode)
        {
            mousePosition = Utilities.GetMouseWorldPosition();
            transform.position = Vector3.Lerp(transform.position, BuildingSystem.Instance.SnapCoordinateToGrid(mousePosition), Time.deltaTime * dragSpeed);
        }

        // Cancel build
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape) && buildMode)
        {
            BuildingSystem.OnBuildMode?.Invoke(false);
            Destroy(gameObject);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0) && buildMode)
        {
            BuildMultiple();
        } 
        else if (Input.GetKeyDown(KeyCode.Mouse0) && buildMode)
        {
            BuildOnce();
        }

    }

    #endregion

    #region Methods

    private void BuildOnce()
    {
        // Do nothing if over UI
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        if (pointerEventData.selectedObject)
        {
            return;
        }

        transform.position = BuildingSystem.Instance.SnapCoordinateToGrid(mousePosition);

        Collider2D[] overlapColliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale * 0.97f, 0.0f);
        foreach (Collider2D collider in overlapColliders)
        {
            if (collider.isTrigger || collider.gameObject == gameObject)
            {
                continue;
            }

            // If this is reached, it means we are hitting a collision and will not build here
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

    private void BuildMultiple()
    {
        // Do nothing if over UI
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        if (pointerEventData.selectedObject)
        {
            return;
        }

        transform.position = BuildingSystem.Instance.SnapCoordinateToGrid(mousePosition);

        Collider2D[] overlapColliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale * 0.97f, 0.0f);
        foreach (Collider2D collider in overlapColliders)
        {
            if (collider.isTrigger || collider.gameObject == gameObject)
            {
                continue;
            }

            // If this is reached, it means we are hitting a collision and will not build here
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

        // If not enough resources, don't build more
        if (!ResourceManager.Instance.HasSufficientResources(building.buildingData))
        { 
            return; 
        }

        BuildingSystem.Instance.InitializeWithObject(unit.gameObject);
    }

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
