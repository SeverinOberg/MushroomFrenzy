using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDrag : MonoBehaviour 
{

    #region Variables & Properties

    private Unit unit;
    private BoxCollider2D boxCollider;
    private Building building;
    private SpriteRenderer spriteRenderer;

    private Vector2 mousePosition;

    private bool buildMode;

    private float dragSpeed = 15;

    #endregion

    #region Unity

    private void Awake()
    {
        unit = GetComponent<Unit>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.color = Utilities.DragColor;
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
            Build();
        }

    }

    #endregion

    #region Methods

    private bool Build()
    {
        // Do nothing if mouse is over UI
        PointerEventData pointerEventData = new(EventSystem.current);
        if (pointerEventData.selectedObject)
        {
            return false;
        }

        transform.position = BuildingSystem.Instance.SnapCoordinateToGrid(mousePosition);

        Collider2D[] overlapColliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale * 0.97f, 0.0f);
        foreach (Collider2D collider in overlapColliders)
        {
            if (collider.isTrigger || collider.gameObject == gameObject)
            {
                continue;
            }

            UIGame.LogToScreen($"Can't build here");
            unit.BlinkRed(false);
            return false;
        }

        if (TryGetComponent(out Building building))
        {
            this.building = building;
            ResourceManager.Instance.Wood -= building.buildingData.woodCost;
            ResourceManager.Instance.Stone -= building.buildingData.stoneCost;
        }
        else
        {
            Debug.LogError("Could not find 'Building' component, this is unexpected");
            return false;
        }

        if (TryGetComponent(out Turret turret))
        {
            turret.isSleeping = false;
        }

        if (TryGetComponent(out Mushroom mushroom))
        {
            mushroom.enabled = true;
        }

        unit.StartCoroutine(unit.SetColorDelay(Color.white, 0.3f));

        BuildingSystem.OnBuildMode?.Invoke(false);
        Destroy(this);
        return true;
    }

    private void BuildMultiple()
    {
        if (!Build() || !ResourceManager.Instance.HasSufficientResources(building.buildingData))
        { 
            return; 
        }

        BuildingSystem.Instance.InitializeWithObject(gameObject);
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
