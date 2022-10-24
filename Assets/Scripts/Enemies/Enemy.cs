using System.Collections;
using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;

[System.Serializable]
public class Enemy : Unit
{

    public enum Type
    {
        Boar,
        Goblin,
        Troll
    }

    private Collider2D          collision;
    private Animator            animator;
    private BehaviorTree        behaviorTree;

    private AIPath              aiPath;
    private AIDestinationSetter aiDestinationSetter;
    private Unit                target;
    private Unit                instigator;

    [SerializeField] private DropTable dropTable;

    [SerializeField] private bool  isMelee = true;
    [SerializeField] private bool  isRanged;
    [SerializeField] private float scanDiameter            = 15; 
    [SerializeField] private float meleeAttackRange        = 2.5f;
    [SerializeField] private float attackCooldown          = 2f;
    [SerializeField] private float attackKnockbackForce    = 0;
    [SerializeField] private float minDamage               = 2;
    [SerializeField] private float maxDamage               = 5;
    [SerializeField] private float rangedAttackRange       = 0;
    [SerializeField] private float minRangedAccuracyOffset = 0;
    [SerializeField] private float maxRangedAccuracyOffset = 0;
    [SerializeField] private float minRangedAttackCooldown = 0;
    [SerializeField] private float maxRangedAttackCooldown = 0;
    [SerializeField] private float runAnimationThreshold   = 1;

    public AIPath              AIPath               { get { return aiPath; }              private set { aiPath              = value; } }
    public AIDestinationSetter AIDestinationSetter  { get { return aiDestinationSetter; } private set { aiDestinationSetter = value; } }
    public Unit                Target               { get { return target; }              private set { target              = value; } }
    public Unit                Instigator           { get { return instigator; }          private set { instigator          = value; } }

    public Type enemyType;

    public bool  IsMelee                 { get { return isMelee; }                 private set { isMelee                 = value; } }
    public bool  IsRanged                { get { return isRanged; }                private set { isRanged                = value; } }
    public float ScanDiameter            { get { return scanDiameter; }            private set { scanDiameter            = value; } }
    public float MeleeAttackRange        { get { return meleeAttackRange; }        private set { meleeAttackRange        = value; } }
    public float AttackCooldown          { get { return attackCooldown; }          private set { attackCooldown          = value; } }
    public float AttackKnockbackForce    { get { return attackKnockbackForce; }    private set { attackKnockbackForce    = value; } }
    public float MinDamage               { get { return minDamage; }               private set { minDamage               = value; } }
    public float MaxDamage               { get { return maxDamage; }               private set { maxDamage               = value; } }
    public float RangedAttackRange       { get { return rangedAttackRange; }       private set { rangedAttackRange       = value; } }
    public float MinRangedAccuracyOffset { get { return minRangedAccuracyOffset; } private set { minRangedAccuracyOffset = value; } }
    public float MaxRangedAccuracyOffset { get { return maxRangedAccuracyOffset; } private set { maxRangedAccuracyOffset = value; } }
    public float MinRangedAttackCooldown { get { return minRangedAttackCooldown; } private set { minRangedAttackCooldown = value; } }
    public float MaxRangedAttackCooldown { get { return maxRangedAttackCooldown; } private set { maxRangedAttackCooldown = value; } }
    public float RunAnimationThreshold   { get { return runAnimationThreshold; }   private set { runAnimationThreshold   = value; } }

    protected override void Awake()
    {
        base.Awake();
        collision = GetComponent<Collider2D>();
        animator  = spriteRenderer.GetComponent<Animator>();

        aiPath              = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        behaviorTree        = GetComponent<BehaviorTree>();

    }

    private void OnEnable()
    {
        OnSetMovementSpeed += OnSetMovementSpeedCallback;
    }

    private void OnDisable()
    {
        OnSetMovementSpeed -= OnSetMovementSpeedCallback;
    }

    protected override void Start()
    {
        base.Start();

        type = UnitTypes.Enemy;
        aiPath.maxSpeed = MovementSpeed;
    }

    private void Update()
    {
        ForceReduceVelocity();

        if (IsDead || GameManager.Instance.HasLost || GameManager.Instance.HasWon)
        {
            behaviorTree.enabled = false;
            SetPathingActive(false);
            enabled = false;
            return;
        }

        HandleMovementAnimation();

        if (AIDestinationSetter.target)
        {
            FlipTowardsTarget();
            HandleStuck();
        }
    }

    #region Methods
    public bool IsWithinMeleeAttackRange()
    {
        if (Vector2.Distance(transform.position, AIDestinationSetter.target.position) <= MeleeAttackRange)
        {
            return true;
        }
        return false;
    }

    public bool IsWithinRangedAttackRange()
    {
        if (Vector2.Distance(transform.position, AIDestinationSetter.target.position) <= RangedAttackRange)
        {
            return true;
        }
        return false;
    }

    private void FlipTowardsTarget()
    {
        if (!AIDestinationSetter.target)
        {
            return;
        }

        bool shouldFlip = transform.position.x > AIDestinationSetter.target.transform.position.x;
        if (shouldFlip)
        {
            spriteRenderer.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            spriteRenderer.transform.localScale = new Vector2(1, 1);
        }
        return;
    }

    public void ClearTarget()
    {
        aiDestinationSetter.target = null;
        Target = null;
    }

    public void ClearInstigator()
    {
        Instigator = null;
    }

    public void SetTarget(Unit target)
    {
        Target = target;
        aiDestinationSetter.target = target.transform;
    }

    public void SetPathingActive(bool value)
    {
        aiDestinationSetter.enabled = value;
        aiPath.enabled              = value;
    }

    public void PausePathing(float pauseForSeconds = 1.0f)
    {
        SetPathingActive(false);
        StartCoroutine(ResumeAIDelay(pauseForSeconds));
    }

    private IEnumerator ResumeAIDelay(float pauseForSeconds)
    {
        yield return new WaitForSeconds(pauseForSeconds);
        SetPathingActive(true);
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

    // Get a random +/- accuracy offset value
    public float GetRangedAccuracyOffset()
    {
        if (Utilities.Roll(50))
        {
            return Random.Range(MinRangedAccuracyOffset, MaxRangedAccuracyOffset);
        }
        else
        {
            return Random.Range(-MinRangedAccuracyOffset, -MaxRangedAccuracyOffset);
        }
        
    }

    public void SetMeleeAttackRange(float value)
    {
        MeleeAttackRange = value;
    }

    public void TriggerAnimation(string value)
    {
        animator.SetTrigger(value);
    }

    private void HandleMovementAnimation()
    {
        animator.SetFloat("Speed", AIPath.velocity.magnitude);
    }

    #region Polymorphism
    public override bool TakeDamage(Unit instigator, float value)
    {
        Instigator = instigator;
        behaviorTree.SendEvent("OnTakeDamage");
        animator.SetTrigger("Take Damage");
        return base.TakeDamage(instigator, value);
    }

    public override void Die(float deathDelaySeconds)
    {
        base.Die(deathDelaySeconds);
        animator.SetTrigger("Die");
        rb.simulated = false;
        dropTable.Drop(transform.position);
    }
    #endregion

    #region Events
    private void OnSetMovementSpeedCallback(float value)
    {
        aiPath.maxSpeed = value;
    }
    #endregion

    #endregion

    #region Archive

    private float isStuckTimer;
    private float isStuckCooldown = 5f;
    private float isStuckDistanceLimit = 0.3f;
    private Vector2 lastStuckEvaluationPosition;

    public void HandleStuck(bool isPatrolling = false)
    {
        isStuckTimer += Time.deltaTime;
        if (isStuckTimer >= isStuckCooldown)
        {
            isStuckTimer = 0;
            if (IsStuck(isPatrolling))
            {
                if (collision.isActiveAndEnabled)
                    collision.enabled = false;
            }
            else
            {
                if (!collision.isActiveAndEnabled)
                    collision.enabled = true;
            }
        }
    }

    private bool IsStuck(bool isPatrolling)
    {
        if (isPatrolling)
        {
            if (HasMovedLessThanStuckLimit())
            {
                return true;
            }
        }
        else
        {
            if (!IsWithinMeleeAttackRange() && !IsWithinRangedAttackRange() && HasMovedLessThanStuckLimit())
            {
                return true;
            }
        }

        lastStuckEvaluationPosition = transform.position;
        return false;
    }

    private bool HasMovedLessThanStuckLimit()
    {
        if (Vector2.Distance(transform.position, lastStuckEvaluationPosition) <= isStuckDistanceLimit)
        {
            return true;
        }

        return false;
    }

    #endregion
}


[System.Serializable]
public class SharedEnemy : SharedVariable<Enemy>
{
    public static implicit operator SharedEnemy(Enemy value) { return new SharedEnemy { Value = value }; }
}
