using System.Collections;
using UnityEngine;
using Pathfinding;
using BehaviorDesigner.Runtime;

[System.Serializable]
public class Enemy : Unit
{
    
    public enum EnemyTypes
    {
        Boar,
        Goblin,
        Troll
    }

    [Header("Enemy")]
    [SerializeField] private EnemySO             enemySO;
    [SerializeField] private DropTable           dropTable;
    [SerializeField] private Collider2D          collision;
    [SerializeField] private Animator            animator;
    [SerializeField] private AIPath              aiPath;
    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private BehaviorTree        behaviorTree;

    private float meleeAttackRange;

    public EnemyTypes EnemyType          { get { return enemySO.type;      } }
    public float      ScanRange          { get { return enemySO.scanRange; } }
    public bool       IsMelee            { get { return enemySO.isMelee;   } }
    public bool       IsRanged           { get { return enemySO.isRanged;  } }

    public float MeleeAttackRange        { get { return meleeAttackRange; } set { meleeAttackRange = value; } }

    public float MeleeAttackSpeed        { get { return enemySO.meleeAttackSpeed;        } }
    public float MinMeleeDamage          { get { return enemySO.minMeleeDamage;          } }
    public float MaxMeleeDamage          { get { return enemySO.maxMeleeDamage;          } }
    public float MeleeKnockbackForce     { get { return enemySO.meleeKnockbackForce;     } }

    public float RangedAttackRange       { get { return enemySO.rangedAttackRange;       } }
    public float RangedAttackSpeed       { get { return enemySO.rangedAttackSpeed;       } }
    public float MinRangedDamage         { get { return enemySO.minRangedDamage;         } }
    public float MaxRangedDamage         { get { return enemySO.maxRangedDamage;         } }
    public float RangedKnockbackForce    { get { return enemySO.rangedKnockbackForce;    } }
    public float MinRangedAccuracyOffset { get { return enemySO.minRangedAccuracyOffset; } }
    public float MaxRangedAccuracyOffset { get { return enemySO.maxRangedAccuracyOffset; } }

    private bool canAttack = true;

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

        InjectSOIntoBackingFields();
        aiPath.maxSpeed = MovementSpeed;
    }

    private void InjectSOIntoBackingFields()
    {
        MeleeAttackRange = enemySO.meleeAttackRange;
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

        if (aiDestinationSetter.target)
        {
            FlipTowardsTarget();
            HandleStuck();
        }
    }

    public bool IsWithinMeleeAttackRange()
    {
        if (Vector2.Distance(transform.position, aiDestinationSetter.target.position) <= MeleeAttackRange)
        {
            return true;
        }
        return false;
    }

    public bool IsWithinRangedAttackRange()
    {
        if (Vector2.Distance(transform.position, aiDestinationSetter.target.position) <= RangedAttackRange)
        {
            return true;
        }
        return false;
    }

    private void FlipTowardsTarget()
    {
        if (!aiDestinationSetter.target)
        {
            return;
        }

        bool shouldFlip = transform.position.x > aiDestinationSetter.target.transform.position.x;
        if (shouldFlip)
        {
            SpriteRenderer.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            SpriteRenderer.transform.localScale = new Vector2(1, 1);
        }
        return;
    }

    public void SetTarget(Unit target)
    {
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

    private void HandleMovementAnimation()
    {
        if (aiPath.canMove)
        {
            animator.SetFloat("movement_speed", aiPath.velocity.magnitude);
        }
        else
        {
            animator.SetFloat("movement_speed", 0);
        }
    }

    public override bool TakeDamage(Unit instigator, float value)
    {
        if (instigator.CompareTag("Player"))
        {
            behaviorTree.SendEvent("OnTakeDamageFromPlayer", (object)instigator.transform);
        }
        
        animator.SetTrigger("take_damage");
        return base.TakeDamage(instigator, value);
    }

    public override void Die(float deathDelaySeconds)
    {
        base.Die(deathDelaySeconds);
        animator.SetTrigger("die");
        Rigidbody.simulated = false;
        dropTable.Drop(transform.position);
    }

    private void OnSetMovementSpeedCallback(float value)
    {
        aiPath.maxSpeed = value;
    }

    public override bool Attack(Unit target)
    {
        if (canAttack)
        {
            canAttack = false;
            StartCoroutine(DoResetCanAttack());

            animator.SetTrigger("attack");

            bool isDead = target.TakeDamage(Utilities.GetRandomFromMinMax(MeleeAttackRange, MeleeAttackRange));
            if (isDead)
            {
                return true;
            }

            target.Blink(Color.red);
            target.AddForce((aiDestinationSetter.target.position - transform.position).normalized, MeleeKnockbackForce);
        }
        return false;
    }

    private IEnumerator DoResetCanAttack()
    {
        yield return new WaitForSeconds(MeleeAttackSpeed);
        canAttack = true;
    }

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
