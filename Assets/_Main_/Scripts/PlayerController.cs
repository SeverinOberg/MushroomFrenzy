using BehaviorDesigner.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Unit
{

    [SerializeField] private InputController inputController;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private BuildingSystem  buildingSystem;
    [SerializeField] private UIManager       uiManager;
    [SerializeField] private Animator        animator;
    [SerializeField] private Animator        effectsAnimator;
    [SerializeField] private Animator        attackEffectsAnimator;
    [SerializeField] private Transform       attackEffectsArm;

    public InputController InputController { get { return inputController; } }
    public ResourceManager ResourceManager { get { return resourceManager; } }
    public BuildingSystem  BuildingSystem  { get { return buildingSystem;  } }
    public UIManager       UIManager       { get { return uiManager;       } }
    public Animator        EffectsAnimator { get { return effectsAnimator; } }

    public float  AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    public float  InteractRange { get { return interactRange; } }

    private float attackDamage  = 1;
    private float interactRange = 5;
    
    private float horizontalInput;
    private float verticalInput;

    private Vector2 movement;
    private bool    stopMovement;
    
    private float timeSinceLastGather;
    private float gatherSpeed = 1f;

    [HideInInspector] public bool axeUpgrade;
    [HideInInspector] public bool pickaxeUpgrade;
    
    private float timeSinceLastAttack;
    private float attackCooldown      = 1.0f;
    private float attackRadius        = 1.8f;
    private float attackDistance      = 0.5f;
    private float offsetPosMultiplier = 1.8f;

    private BehaviorTree interactTargetBT;

    private Vector2 mouseDirectionFromPlayer;
    private float   distanceFromMouse;

    protected override void Start()
    {
        base.Start();
        timeSinceLastGather += timeSinceLastGather;
    }

    private void OnEnable()
    {
        InputController.OnCancel   += OnCancelCallback;
        InputController.OnMouse0   += OnMouse0Callback;
        InputController.OnKeyT     += SellBuilding;
        InputController.OnKeyR     += RepairBuilding;
        InputController.OnKeyV     += UpgradeBuilding;
    }

    private void OnDisable()
    {
        InputController.OnCancel   -= OnCancelCallback;
        InputController.OnMouse0   -= OnMouse0Callback;
        InputController.OnKeyT     -= SellBuilding;
        InputController.OnKeyR     -= RepairBuilding;
        InputController.OnKeyV     -= UpgradeBuilding;
    }

    private void Update()
    {
        //// DEBUG MOUSE RAYCAST HIT
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] results);
        //    for (int i = 0; i < results.Length; i++)
        //    {
        //        Debug.Log($"Raycast: {results[i].transform.name}");
        //    }

        //    // DEBUG MOUSE UI EVENT DATA
        //    PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        //    Debug.Log($"pointerEventData: {pointerEventData.selectedObject}");
        //}
        //// DEBUG

        ForceReduceVelocity();

        if (IsDead || GameManager.Instance.HasLost || GameManager.Instance.HasWon)
        {
            return;
        }


        HandleInteraction();
        HandleTimers();
        HandleMouseDirection();
        HandleFlipPlayer();

        if (!stopMovement)
        {
            HandleMovement();
        }
    }

    #region Automatic Calculations

    private void HandleInteraction()
    {
        if (interactTargetBT && !IsWithinInteractTargetRange())
        {
            EndInteraction();
        }
    }

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
            SpriteRenderer.flipX = true;
        }
        else
        {
            SpriteRenderer.flipX = false;
        }
    }

    private void HandleTimers()
    {
        timeSinceLastGather   += Time.deltaTime;
        timeSinceLastAttack   += Time.deltaTime;
    }

    #endregion

    private void OnMouse0Callback()
    {
        if (timeSinceLastGather >= gatherSpeed)
        {
            Utilities.GetRaycastAllOnMousePoint(out RaycastHit2D[] result);
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i].collider.CompareTag("Resource Node") && IsWithinMouseInteractRange())
                {
                    ResourceNode.ResourceType gatherType = result[i].collider.GetComponent<ResourceNode>().Type;
                    if (axeUpgrade && gatherType == ResourceNode.ResourceType.Wood)
                    {
                        timeSinceLastGather = gatherSpeed * 0.5f;
                    }
                    else if (pickaxeUpgrade && gatherType == ResourceNode.ResourceType.Stone || pickaxeUpgrade && gatherType == ResourceNode.ResourceType.IronOre)
                    {
                        timeSinceLastGather = gatherSpeed * 0.5f;
                    }
                    else
                    {
                        timeSinceLastGather = gatherSpeed * 0 ;
                    }
                    
                    Gather(result[i].collider.GetComponent<ResourceNode>());
                    return;
                }

                if (result[i].collider.CompareTag("Merchant") && IsWithinMouseInteractRange())
                {
                    interactTargetBT = result[i].collider.GetComponent<BehaviorTree>();
                    interactTargetBT.SendEvent("InteractionStart", (object)transform);
                
                    UIManager.SetMerchantUIActive(true);
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

    public override void Attack()
    {
        if (timeSinceLastAttack <= attackCooldown || BuildingSystem.BuildMode ||  IsMouseOverSelectableTarget())
        {
            return;
        }

        Utilities.ResetTimer(ref timeSinceLastAttack);

        Vector2 offsetPos = new Vector2
        (
            transform.position.x + mouseDirectionFromPlayer.x * offsetPosMultiplier,
            transform.position.y + mouseDirectionFromPlayer.y * offsetPosMultiplier
        );

        attackEffectsArm.up = mouseDirectionFromPlayer;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        if (pointerEventData.selectedObject)
        {
            return;
        }

        attackEffectsAnimator.SetTrigger("Attack");
        animator.SetTrigger("Attack");

        RaycastHit2D[] hits = Physics2D.CircleCastAll(offsetPos, attackRadius, mouseDirectionFromPlayer, attackDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].transform.CompareTag("Enemy"))
            {
                continue;
            }
            
            if (hits[i].transform.TryGetComponent(out Enemy enemy) && !enemy.IsDead)
            {
                enemy.TakeDamage(this, Random.Range(AttackDamage * 0.5f, AttackDamage * 2));
                enemy.Blink(Color.red);
                enemy.AddForce(mouseDirectionFromPlayer, 10);
                enemy.PausePathing(0.2f);
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
                if (!IsWithinMouseInteractRange())
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
                if (!IsWithinMouseInteractRange())
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
                if (!IsWithinMouseInteractRange())
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

    public bool IsWithinMouseInteractRange(bool logToScreen = true)
    {
        distanceFromMouse = Vector2.Distance(transform.position, Utilities.GetMouseWorldPosition());
        if (distanceFromMouse >= InteractRange)
        {
            if (logToScreen)
                UIManager.LogToScreen($"Too far away");

            return false;
        }

        return true;
    }

    public bool IsWithinInteractTargetRange()
    {
        if (Utilities.GetDistanceBetween(transform.position, interactTargetBT.transform.position) >= InteractRange)
        {
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

    private void OnCancelCallback()
    {
        EndInteraction();
    }

    private void EndInteraction()
    {
        UIManager.SetMerchantUIActive(false);
        UIManager.SetSmelterUIActive(false);

        if (interactTargetBT)
        {
            interactTargetBT.SendEvent("InteractionEnd");
            interactTargetBT = null;
        }
    }

    public override void Die(float destroyDelaySeconds)
    {
        base.Die(destroyDelaySeconds);
        stopMovement = true;
        animator.SetTrigger("Dead");
        GameManager.Instance.LoseGame();
    }

}
