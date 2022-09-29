using System.Timers;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Unit 
{

    #region Variables & Properties

    [SerializeField] Animator animator;
    [SerializeField] Animator effectsAnimator;

    [SerializeField] Transform arm;

    InputController inputController;

    private float horizontalInput;
    private float verticalInput;

    private Vector2 movement;

    private float speed = 7;

    private float timeSinceLastAttack;
    private float attackCooldown = 1.0f;
    private float attackRadius = 1.8f;
    private float attackDistance = 0.5f;
    private float offsetPosMultiplier = 1.8f;
    private float attackDamage = 3;
    private float interactRange = 5f;

    private bool buildMode;
    private Rigidbody2D rb;

    Vector2 mouseDirectionFromPlayer;
    float distanceFromMouse;

    #endregion

    #region Unity

    protected override void Awake()
    {
        base.Awake();

        inputController = GetComponent<InputController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        inputController.OnMouse0   += Attack;
        inputController.OnKeyG     += TakeDamage;
        inputController.OnKeyT     += DestroyBuilding;
        inputController.OnKeyR     += RepairBuilding;
        BuildingSystem.OnBuildMode += (value) => buildMode = value;
    }

    private void OnDisable()
    {
        inputController.OnMouse0   -= Attack;
        inputController.OnKeyG     -= TakeDamage;
        BuildingSystem.OnBuildMode -= (value) => buildMode = value;
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseDirection();
        HandleFlipPlayer();
        ForceReduceVelocity();
        HandleTimer();
    }

    #endregion

    #region Methods

    #region Automatic Calculations

    
    private void HandleMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized;

        transform.Translate(movement * speed * Time.deltaTime);

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

    // Force reduce velocity to keep Player from gliding
    private void ForceReduceVelocity()
    {
        if (rb.velocity.normalized != Vector2.zero)
        {
            rb.velocity = rb.velocity * 0.95f;
        }
    }

    private void HandleTimer()
    {
        timeSinceLastAttack += Time.deltaTime;
    }

    #endregion

    private void Attack()
    {
        if (buildMode || timeSinceLastAttack <= attackCooldown)
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
        foreach (RaycastHit2D hit in hits)
        {

            if (hit.transform.CompareTag("Enemy") == false)
            {
                continue;
            }

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Random.Range(attackDamage / 2, attackDamage * 2));

                if (!enemy.isDead)
                {
                    enemy.rb.AddForce(mouseDirectionFromPlayer * 10, ForceMode2D.Impulse);
                    enemy.BlinkRed();
                }
                
                enemy.PauseAI(0.2f);
            }
        }
    }

    private void TakeDamage()
    {
        TakeDamage(10);
    }

    private void DestroyBuilding()
    {
        RaycastHit2D[] hits = Utilities.GetRaycastAllOnMousePoint();
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.TryGetComponent(out Building building) && !building.isDead) 
            {
                if (!IsPlayerWithinInteractRange())
                {
                    return;
                }

                // If the building has taken damage, the resources returned will be halfed
                if (building.health < building.unitData.health)
                {
                    ResourceManager.Instance.Wood += building.buildingData.woodCost / 2;
                    ResourceManager.Instance.Stone += building.buildingData.stoneCost / 2;
                } else
                {
                    ResourceManager.Instance.Wood += building.buildingData.woodCost;
                    ResourceManager.Instance.Stone += building.buildingData.stoneCost;
                }

                Destroy(building.gameObject);
            }
        }
    }

    private void RepairBuilding()
    {
        RaycastHit2D[] hits = Utilities.GetRaycastAllOnMousePoint();
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger)
            { 
                continue;
            }

            if (hit.transform.TryGetComponent(out Building building) && !building.isDead)
            {
                if (!IsPlayerWithinInteractRange())
                {
                    return;
                }

                if (!ResourceManager.Instance.HasSufficientResources(building.buildingData))
                {
                    Debug.Log("Not enough resources to repair this building");
                    return;
                }

                // If the building has taken damage, the resources returned will be halfed
                if (building.health >= building.unitData.health)
                {
                    Debug.Log("Building is already fully repaired");
                    return;
                }

                // Repairing always costs at least 1 or half (rounded away from zero) the price to build it
                ResourceManager.Instance.Wood  -= (int)System.Math.Round((decimal)building.buildingData.woodCost / 2, System.MidpointRounding.AwayFromZero);
                ResourceManager.Instance.Stone -= (int)System.Math.Round((decimal)building.buildingData.stoneCost / 2, System.MidpointRounding.AwayFromZero);

                // Repairing always heals half the buildings max health
                building.Heal(building.unitData.health / 2);
            }
        }
    }

    private bool IsPlayerWithinInteractRange()
    {
        distanceFromMouse = Vector2.Distance(transform.position, Utilities.GetMouseWorldPosition());
        if (distanceFromMouse >= interactRange)
        {
            // @TODO: Move debug to infoText instead
            Debug.Log("Too far away from building to destroy it, try to move closer.");
            return false;
        }

        return true;
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("Dead");
    }

    #endregion
}
