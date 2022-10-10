using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDrag : MonoBehaviour 
{

    #region Variables & Properties

    private Unit unit;
    private Building building;
    private BoxCollider2D boxCollider;
    private BoxCollider2D boxTrigger;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private int initialSpriteRendererSortingOrder;

    private Vector2 mousePosition;

    private bool buildMode;

    private float dragSpeed = 15;

    #endregion

    #region Unity

    private void Awake()
    {
        unit = GetComponent<Unit>();

        BoxCollider2D[] boxColliders = GetComponents<BoxCollider2D>();
        for (int i = 0; i < boxColliders.Length; i++)
        {
            if (boxColliders[i].isTrigger)
            {
                boxTrigger = boxColliders[i];
            }
            else
            {
                boxCollider = boxColliders[i];
            }
        }
        if (!TryGetComponent(out spriteRenderer))
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (!TryGetComponent(out animator))
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        if (boxTrigger)
            boxTrigger.enabled = false;
        if (boxCollider)
            boxCollider.enabled = false;

        animator.enabled = false;
        initialSpriteRendererSortingOrder = spriteRenderer.sortingOrder;
        spriteRenderer.sortingOrder = 4;
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
            if (!CanBuildHere())
            {
                spriteRenderer.color = Color.red;
            } 
            else
            {
                spriteRenderer.color = Utilities.DragColor;
            }
        }

        // Cancel
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape) && buildMode)
        {
            BuildingSystem.OnBuildMode?.Invoke(false);
            Destroy(gameObject);
        }

        // Build
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

        if (!CanBuildHere())
        {
            UIGame.LogToScreen("Can't build here");
            return false;
        }

        if (TryGetComponent(out Building building))
        {
            this.building = building;
            ResourceManager.Instance.PayForBuild(building.buildingData);
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
        spriteRenderer.sortingOrder = initialSpriteRendererSortingOrder;

        animator.enabled = true;

        if (boxTrigger)
            boxTrigger.enabled = true;
        if (boxCollider)
            boxCollider.enabled = true;

        BuildingSystem.OnBuildMode?.Invoke(false);
        Destroy(this);
        return true;
    }

    private void BuildMultiple()
    {
        if (!Build() || !ResourceManager.Instance.HasSufficientResourcesToBuild(building.buildingData))
        {
            return; 
        }


        if (!TryGetComponent(out Unit unit))
        {
            Debug.LogError("Could not find unit for the rebuild");
            return;
        }

        BuildingSystem.Instance.InitializeWithObject(unit.unitData.prefab);
    }

    public void SetBuildMode(bool value)
    {
        buildMode = value;
        //if (buildMode)
        //{
        //    boxCollider.enabled = false;
        //}
        //else
        //{
        //    boxCollider.enabled = true;
        //}
    }

    private bool CanBuildHere()
    {
        Collider2D[] overlapColliders = Physics2D.OverlapBoxAll(transform.position, boxCollider.size * 0.97f, 0.0f);
        foreach (Collider2D collider in overlapColliders)
        {
            if (collider.isTrigger || collider.gameObject == gameObject)
            {
                continue;
            }
            return false;
        }

        return true;
    }

    #endregion

}
