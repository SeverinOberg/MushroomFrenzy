using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Unit
{

    #region Variables & Properties

    [HideInInspector] public ResourceManager resourceManager;
    [HideInInspector] public BuildingSystem buildingSystem;

    [SerializeField] private Animator effectsAnimator;
    [SerializeField] private Transform arm;

    private InputController inputController;
    private Animator animator;

    private float horizontalInput;
    private float verticalInput;

    private Vector2 movement;
    private bool stopMovement;

    private float timeSinceLastAttack;
    private float attackCooldown = 1.0f;
    private float attackRadius = 1.8f;
    private float attackDistance = 0.5f;
    private float offsetPosMultiplier = 1.8f;
    private float attackDamage = 3;

    private float interactCooldown = 1.5f;
    private float timeSinceLastInteract;
    public float InteractRange { get; private set; } = 5f;

    private Vector2 mouseDirectionFromPlayer;
    private float   distanceFromMouse;

    #endregion

    #region Unity

    protected override void Awake()
    {
        base.Awake();
        resourceManager = GetComponent<ResourceManager>();
        buildingSystem  = GetComponent<BuildingSystem>();
        inputController = GetComponent<InputController>();
        animator        = spriteRenderer.GetComponent<Animator>();

        type = UnitTypes.Player;

        MovementSpeed = 7;
    }

    private void OnEnable()
    {
        inputController.OnMouse0   += OnMouse0Callback;
        inputController.OnKeyT     += SellBuilding;
        inputController.OnKeyR     += RepairBuilding;
        inputController.OnKeyV     += UpgradeBuilding;
    }

    private void OnDisable()
    {
        inputController.OnMouse0   -= OnMouse0Callback;
        inputController.OnKeyT     -= SellBuilding;
        inputController.OnKeyR     -= RepairBuilding;
        inputController.OnKeyV     -= UpgradeBuilding;
    }

    private void Update()
    {
        ForceReduceVelocity();
        HandleTimer();
        HandleMouseDirection();
        HandleFlipPlayer();

        if (!stopMovement)
        {
            HandleMovement();
        }
    }

    #endregion

    #region Methods

    #region Automatic Calculations

    
    private void HandleMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized;

        transform.Translate(movement * MovementSpeed * Time.deltaTime);

        if (horizontalInput != 0 || verticalInput != 0)
        {
            animator.SetFloat("Run", 1);
        }
        else
        {
            animator.SetFloat("Run", 0);
        }
    }

    private void HandleMouseDirection()
    {
        mouseDirectionFromPlayer = ((Vector2)Utilities.GetMouseWorldPosition() - (Vector2)transform.position).normalized;
    }

    private void HandleFlipPlayer()
    {
        if (mouseDirectionFromPlayer.x < 0 || horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void HandleTimer()
    {
        timeSinceLastInteract += Time.deltaTime;
        timeSinceLastAttack += Time.deltaTime;
    }

    #endregion

    private void OnMouse0Callback()
    {
        if (timeSinceLastInteract >= interactCooldown)
        {
            Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] result);
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].collider.CompareTag("Resource Node") && IsPlayerWithinInteractRange())
                {
                    timeSinceLastInteract = 0;
                    Gather(result[i].collider.GetComponent<ResourceNode>());
                    return;
                }
            }
            Attack();
        }
    }

    private void Gather(ResourceNode resourceNode)
    {
        animator.SetTrigger("Attack");
        resourceNode.Gather();
    }

    private void Attack()
    {
        if (timeSinceLastAttack <= attackCooldown || buildingSystem.BuildMode ||  IsMouseOverSelectableTarget())
        {
            return;
        }

        Utilities.ResetTimer(ref timeSinceLastAttack);

        Vector2 offsetPos = new Vector2
        (
            transform.position.x + mouseDirectionFromPlayer.x * offsetPosMultiplier,
            transform.position.y + mouseDirectionFromPlayer.y * offsetPosMultiplier
        );

        arm.up = mouseDirectionFromPlayer;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        if (pointerEventData.selectedObject)
        {
            return;
        }

        effectsAnimator.SetTrigger("Attack");
        animator.SetTrigger("Attack");

        RaycastHit2D[] hits = Physics2D.CircleCastAll(offsetPos, attackRadius, mouseDirectionFromPlayer, attackDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].transform.CompareTag("Enemy"))
            {
                continue;
            }
            
            if (hits[i].transform.TryGetComponent(out Unit unit) && !unit.IsDead)
            {
                unit.TakeDamage(this, Random.Range(attackDamage * 0.5f, attackDamage * 2));
                unit.Blink(Color.red);
                if (unit.TryGetComponent(out Enemy enemy))
                {
                    enemy.AddForce(mouseDirectionFromPlayer, 10);
                    enemy.PausePathing(0.2f);
                }
            }
        }
    }

    public override bool TakeDamage(float value)
    {
        return base.TakeDamage(value);
    }

    private void SellBuilding()
    {
        if (!Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] hits))
        {
            return;
        }

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.TryGetComponent(out Building building) && !building.IsDead) 
            {
                if (!IsPlayerWithinInteractRange())
                {
                    return;
                }

                building.SellBuilding();
            }
        }
    }

    private void RepairBuilding()
    {
        if (!Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] hits))
        {
            return;
        }

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger)
            { 
                continue;
            }

            if (hit.transform.TryGetComponent(out Building building) && !building.IsDead)
            {
                if (!IsPlayerWithinInteractRange())
                {
                    return;
                }

                building.RepairBuilding();
            }
        }
    }

    private void UpgradeBuilding()
    {
        if (!Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] hits))
        {
            return;
        }

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger)
            {
                continue;
            }

            if (hit.transform.TryGetComponent(out Building building) && !building.IsDead)
            {
                if (!IsPlayerWithinInteractRange())
                {
                    return;
                }

                building.UpgradeBuilding();
            }
        }
    }

    private bool IsMouseOverSelectableTarget()
    {
        if (Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] hits))
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.TryGetComponent(out Building selectableTarget) && !selectableTarget.IsDead)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsPlayerWithinInteractRange()
    {
        distanceFromMouse = Vector2.Distance(transform.position, Utilities.GetMouseWorldPosition());
        if (distanceFromMouse >= InteractRange)
        {
            UIGame.LogToScreen($"Too far away");
            return false;
        }

        return true;
    }

    public void SetStopMovement(bool value, float seconds = 0)
    {
        if (value == true)
        {
            stopMovement = true;
            if (seconds > 0)
            {
                StartCoroutine(DoClearStopMovement(seconds));
            }
        }
        else
        {
            stopMovement = false;
        }
    }

    private IEnumerator DoClearStopMovement(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        stopMovement = false;
    }

    public void TriggerAnimation(string trigger)
    {
        animator.SetTrigger(trigger);
    }

    public void SetAnimationBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    public override void Die(float destroyDelaySeconds)
    {
        base.Die(destroyDelaySeconds);
        stopMovement = true;
        animator.SetTrigger("Dead");
    }

    #endregion
}
