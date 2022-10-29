using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingDrag : MonoBehaviour 
{

    private PlayerController owner;

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

    private bool shouldRotate;


    private void Awake()
    {
        building = GetComponent<Building>();
        owner    = building.GetOwner();

        unit     = GetComponent<Unit>();

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

    private void OnEnable()
    {
        owner.inputController.OnKeyR += OnKeyRCallback;
    }

    private void OnDisable()
    {
        owner.inputController.OnKeyR -= OnKeyRCallback;
    }

    private void LateUpdate()
    {
        // Drag
        if (owner.buildingSystem.BuildMode)
        {
            mousePosition = Utilities.GetMouseWorldPosition();
            transform.position = Vector3.Lerp(transform.position, owner.buildingSystem.SnapCoordinateToGrid(mousePosition), Time.deltaTime * dragSpeed);

            if (shouldRotate)
            {
                shouldRotate = false;

                if (transform.localScale.x == 1)
                {
                    transform.localScale = new Vector2(-1, 1);
                }
                else
                {
                    transform.localScale = new Vector2(1, 1);
                }
            }

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
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape) && owner.buildingSystem.BuildMode)
        {
            owner.buildingSystem.BuildMode = false;
            Destroy(gameObject);
        }

        // Build
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0) && owner.buildingSystem.BuildMode)
        {
            BuildMultiple();
        } 
        else if (Input.GetKeyDown(KeyCode.Mouse0) && owner.buildingSystem.BuildMode)
        {
            Build();
        }

    }

    private void OnKeyRCallback()
    {
        if (owner.buildingSystem.BuildMode)
            shouldRotate = true;
    }

    private bool Build()
    {
        // Do nothing if mouse is over UI
        PointerEventData pointerEventData = new(EventSystem.current);
        if (pointerEventData.selectedObject)
        {
            return false;
        }

        transform.position = owner.buildingSystem.SnapCoordinateToGrid(mousePosition);

        if (!CanBuildHere())
        {
            UIManager.LogToScreen("Can't build here");
            return false;
        }

        if (!owner.resourceManager.PayForBuild(building.buildingData))
        {
            return false;
        }
 
        if (TryGetComponent(out Turret turret))
        {
            turret.isSleeping = false;
        }

        if (TryGetComponent(out MendingMushroom mendingMushroom))
        {
            mendingMushroom.enabled = true;
        }

        unit.Blink(Color.green, default, true);
        unit.StartCoroutine(unit.DoSetColorDelay(Color.white, 0.3f));
        spriteRenderer.sortingOrder = initialSpriteRendererSortingOrder;

        if (animator)
            animator.enabled    = true;
        if (boxTrigger)
            boxTrigger.enabled  = true;
        if (boxCollider)
            boxCollider.enabled = true;

        if (!unit.isActiveAndEnabled)
            unit.enabled = true;

        owner.buildingSystem.BuildMode = false;
        Destroy(this);
        return true;
    }

    private void BuildMultiple()
    {
        if (!Build() || !owner.resourceManager.HasSufficientResourcesToBuild(building.buildingData, out _))
        {
            return; 
        }

        owner.buildingSystem.InitializeWithObject(building.unitData.prefab);
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

}
