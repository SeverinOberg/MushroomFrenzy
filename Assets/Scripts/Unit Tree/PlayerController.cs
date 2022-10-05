using System.Collections;
using System.Timers;
using Unity.VisualScripting;
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
    private bool stopMovement;

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

        type = UnitTypes.Player;
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

    protected override void Update()
    {
        Utilities.ForceReduceVelocity(ref rb);
        HandleTimer();
        HandleMouseDirection();
        HandleFlipPlayer();

        if (!stopMovement)
        {
            HandleMovement();
        }

        base.Update();
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

            EnemyBT enemy = hit.transform.GetComponent<EnemyBT>();
            if (enemy != null)
            {
                enemy.TakeDamage(Random.Range(attackDamage * 0.5f, attackDamage * 2));

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
        BlinkRed();
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
                    ResourceManager.Instance.Wood += (int)(building.buildingData.woodCost * 0.5f);
                    ResourceManager.Instance.Stone += (int)(building.buildingData.stoneCost * 0.5f);
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
                    return;
                }

                // If the building has taken damage, the resources returned will be halfed
                if (building.health >= building.unitData.health)
                {
                    UIGame.LogToScreen($"Already fully repaired");
                    return;
                }

                // Repairing always costs at least 1 or half (rounded away from zero) the price to build it
                ResourceManager.Instance.Wood  -= (int)System.Math.Round((decimal)building.buildingData.woodCost  / 2, System.MidpointRounding.AwayFromZero);
                ResourceManager.Instance.Stone -= (int)System.Math.Round((decimal)building.buildingData.stoneCost / 2, System.MidpointRounding.AwayFromZero);

                // Repairing always heals half the buildings max health
                building.Heal(building.unitData.health * 0.5f);
            }
        }
    }

    private bool IsPlayerWithinInteractRange()
    {
        distanceFromMouse = Vector2.Distance(transform.position, Utilities.GetMouseWorldPosition());
        if (distanceFromMouse >= interactRange)
        {
            UIGame.LogToScreen($"Too far away");
            return false;
            }

        return true;
    }

    public override void Die()
    {
        base.Die();
        stopMovement = true;
        animator.SetTrigger("Dead");
    }

    public void SetStopMovement(bool value, float seconds = 0)
    {
        if (value == true)
        {
            stopMovement = true;
            if (seconds > 0)
            {
                StartCoroutine(StartMovementRoutine(seconds));
            }
        }
        else
        {
            stopMovement = false;
        }
        
    }

    private IEnumerator StartMovementRoutine(float seconds)
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

    #endregion
}
