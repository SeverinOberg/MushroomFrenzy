using System.Collections;
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
        inputController.OnKeyT     += SellBuilding;
        inputController.OnKeyR     += RepairBuilding;
        inputController.OnKeyV     += UpgradeBuilding;
    }

    private void OnDisable()
    {
        inputController.OnMouse0   -= Attack;
        inputController.OnKeyT     -= SellBuilding;
        inputController.OnKeyR     -= RepairBuilding;
        inputController.OnKeyV     -= UpgradeBuilding;
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
        if (BuildingSystem.Instance.buildMode || timeSinceLastAttack <= attackCooldown)
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
                unit.TakeDamage(Random.Range(attackDamage * 0.5f, attackDamage * 2));
                unit.Blink(Color.red);
                if (unit.TryGetComponent(out EnemyBT enemy))
                {
                    enemy.rb.AddForce(mouseDirectionFromPlayer * 10, ForceMode2D.Impulse);
                    enemy.PauseAI(0.2f);
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
