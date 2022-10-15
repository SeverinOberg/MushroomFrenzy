using System.Collections;
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

    private float dragSpeed = 15;

    private bool  pauseCanBuildHereValuation;
    private float clearCanBuildHereValuationDuration = 0.25f;

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
            boxTrigger.enabled  = false;
        if (boxCollider)
            boxCollider.enabled = false;
        if (animator)
            animator.enabled    = false;

        initialSpriteRendererSortingOrder = spriteRenderer.sortingOrder;
        spriteRenderer.sortingOrder = 4;
        spriteRenderer.color = Utilities.DragColor;

        pauseCanBuildHereValuation = true;
        StartCoroutine(DoClearPauseCanBuildHereValuation());
    }

    private void LateUpdate()
    {
        // Drag
        if (BuildingSystem.Instance.buildMode)
        {
            mousePosition = Utilities.GetMouseWorldPosition();
            transform.position = Vector3.Lerp(transform.position, BuildingSystem.Instance.SnapCoordinateToGrid(mousePosition), Time.deltaTime * dragSpeed);

            if (!pauseCanBuildHereValuation && !CanBuildHere())
            {
                spriteRenderer.color = Color.red;
            }
            else
            {
                spriteRenderer.color = Utilities.DragColor;
            }
            
        }

        // Cancel
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape) && BuildingSystem.Instance.buildMode)
        {
            BuildingSystem.Instance.buildMode = false;
            Destroy(gameObject);
        }

        // Build
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0) && BuildingSystem.Instance.buildMode)
        {
            BuildMultiple();
        } 
        else if (Input.GetKeyDown(KeyCode.Mouse0) && BuildingSystem.Instance.buildMode)
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

        unit.Blink(Color.green, true);
        unit.StartCoroutine(unit.SetColorDelay(Color.white, 0.3f));
        spriteRenderer.sortingOrder = initialSpriteRendererSortingOrder;

        if (animator)
            animator.enabled    = true;
        if (boxTrigger)
            boxTrigger.enabled  = true;
        if (boxCollider)
            boxCollider.enabled = true;

        BuildingSystem.Instance.buildMode = false;
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

    private IEnumerator DoClearPauseCanBuildHereValuation()
    {
        yield return new WaitForSeconds(clearCanBuildHereValuationDuration);
        pauseCanBuildHereValuation = false;
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
