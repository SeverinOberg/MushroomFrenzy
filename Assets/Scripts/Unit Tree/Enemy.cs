using System.Collections;
using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;

[System.Serializable]
public class Enemy : Unit
{
    public float scanDiameter           = 15;
    public float meleeAttackRange       = 2.5f;
    public float attackCooldown         = 2f;
    public float minDamage              = 2;
    public float maxDamage              = 5;

    public float ScanDiameter     { get; private set; }
    public float MeleeAttackRange { get; private set; }
    public float AttackCooldown   { get; private set; }
    public float MinDamage        { get; private set; }
    public float MaxDamage        { get; private set; }

    [HideInInspector] public Animator animator;
    [HideInInspector] public Collider2D collision;
    [HideInInspector] public Rigidbody2D rb;

    [HideInInspector] public AIDestinationSetter aiDestinationSetter;
    [HideInInspector] public AIPath aiPath;
    [HideInInspector] private BehaviorTree behaviorTree;

    public Unit target;

    public Unit instigatorTarget;

    public System.Action OnDisableAction;

    protected override void Awake()
    {
        base.Awake();

        animator = spriteRenderer.GetComponent<Animator>();
        collision = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        behaviorTree = GetComponent<BehaviorTree>();
    }

    private void OnEnable()
    {
        OnSetMovementSpeed += OnMovementSpeedChangedCallback;
    }

    private void OnDisable()
    {
        OnDisableAction?.Invoke();
        OnSetMovementSpeed -= OnMovementSpeedChangedCallback;
    }

    protected override void Start()
    {
        base.Start();

        type = UnitTypes.Enemy;
        aiPath.maxSpeed = MovementSpeed;
    }

    private void Update()
    {
        HandleRunningAnimation();
        Utilities.ForceReduceVelocity(ref rb);

        // Stop AI and block Behaviour Tree Evaluations (AI updates) if IsDead
        if (IsDead)
        {
            SetAIState(false);
            return;
        }

        if (target)
        {
            FlipTowardsTarget();
        }
    }

    #region Methods

    public bool IsWithinMeleeAttackRange()
    {
        if (Vector2.Distance(transform.position, target.transform.position) <= meleeAttackRange)
        {
            return true;
        }
        return false;
    }

    public void FlipTowardsTarget()
    {
        if (!aiDestinationSetter.target)
        {
            return;
        }

        if (aiDestinationSetter.target.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    public void ClearTarget()
    {
        aiDestinationSetter.target = null;
        target = null;
    }

    public void AddForce(Vector2 direction, float forceMultiplier)
    {
        rb.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
    }

    public void SetTarget(Unit target)
    {
        this.target = target;
        aiDestinationSetter.target = target.transform;
    }

    public void SetAIState(bool state)
    {
        aiDestinationSetter.enabled = state;
        aiPath.enabled = state;
    }

    public void PauseAI(float pauseForSeconds = 1.0f)
    {
        SetAIState(false);
        StartCoroutine(ResumeAIDelay(pauseForSeconds));
    }

    private IEnumerator ResumeAIDelay(float pauseForSeconds)
    {
        yield return new WaitForSeconds(pauseForSeconds);
        SetAIState(true);
    }

    private void HandleRunningAnimation()
    {
        if (aiPath.velocity.magnitude >= 1)
        {
            animator.SetFloat("Run", 1);
        }
        else
        {
            animator.SetFloat("Run", 0);
        }
    }

    private void SpawnResource()
    {
        GameObject[] randomResources = ResourceManager.Instance.resources;
        GameObject resource = randomResources[Random.Range(0, randomResources.Length)];
        Instantiate(resource, transform.position, Quaternion.identity);
    }

    private void OnMovementSpeedChangedCallback(float value)
    {
        aiPath.maxSpeed = value;
    }

    public bool IsPathPossible(Vector2 from, Vector2 to)
    {
        GraphNode fromNode = AstarPath.active.GetNearest(from, NNConstraint.Default).node;
        GraphNode toNode = AstarPath.active.GetNearest(to, NNConstraint.Default).node;

        if (PathUtilities.IsPathPossible(fromNode, toNode))
        {
            return true;
        }

        return false;
    }

    public override bool TakeDamage(Unit instigator, float value)
    {
        this.instigatorTarget = instigator;
        behaviorTree.SendEvent("OnTakeDamage");
        animator.SetTrigger("Take Damage");
        return base.TakeDamage(instigator, value);
    }

    public override void Die(float deathDelaySeconds)
    {
        base.Die(deathDelaySeconds);
        animator.SetTrigger("Die");
        rb.simulated = false;
        if (Utilities.Roll(33))
        {
            SpawnResource();
        }
    }

    #endregion

    #region Archive

    //private float isStuckTimer;
    //private float isStuckCooldown = 2f;
    //private float isStuckDistanceLimit = 0.3f;
    //private Vector2 lastStuckEvaluationPosition;

    //public void HandleStuck(bool isPatrolling = false)
    //{
    //    isStuckTimer += Time.deltaTime;
    //    if (isStuckTimer >= isStuckCooldown)
    //    {
    //        isStuckTimer = 0;
    //        if (IsStuck(isPatrolling))
    //        {
    //            if (collision.isActiveAndEnabled)
    //                collision.enabled = false;
    //        }
    //        else
    //        {
    //            if (!collision.isActiveAndEnabled)
    //                collision.enabled = true;
    //        }
    //    }
    //}

    //private bool IsStuck(bool isPatrolling)
    //{
    //    if (isPatrolling)
    //    {
    //        if (HasMovedLessThanStuckLimit())
    //        {
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        if (!IsWithinMeleeAttackRange() && HasMovedLessThanStuckLimit())
    //        {
    //            return true;
    //        }
    //    }

    //    lastStuckEvaluationPosition = transform.position;
    //    return false;
    //}

    //private bool HasMovedLessThanStuckLimit()
    //{
    //    if (Vector2.Distance(transform.position, lastStuckEvaluationPosition) <= isStuckDistanceLimit)
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    #endregion
}


[System.Serializable]
public class SharedEnemy : SharedVariable<Enemy>
{
    public static implicit operator SharedEnemy(Enemy value) { return new SharedEnemy { Value = value }; }
}
