using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using Pathfinding;

public class EnemyBT : Unit
{
    public float scanRadius         = 15;
    public float meleeAttackRange   = 2.5f;
    public float attackCooldown     = 2f;
    public float minDamage          = 2;
    public float maxDamage          = 5;

    private float   isStuckTimer;
    private float   isStuckCooldown = 1.5f;
    private float   isStuckDistanceLimit = 0.2f;
    private Vector2 lastStuckEvaluationPosition;

    [HideInInspector] public Animator            animator            { get; set; }
    [HideInInspector] public Collider2D          collision           { get; set; }
    [HideInInspector] public Unit                target              { get; set; }
    [HideInInspector] public AIDestinationSetter aiDestinationSetter { get; set; }

    protected override Node SetupTree()
    {
        animator            = GetComponent<Animator>();
        collision           = GetComponent<Collider2D>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();

        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckEnemyInMeleeRange(this),
                new TaskMeleeAttack(this),
            }),

            new Sequence(new List<Node>
            {
                new CheckFOVScan(this),
                new TaskChase(this),
            }),

            new TaskPatrol(this),
        });

        return root;
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
            if (!IsWithinMeleeAttackRange() && HasMovedLessThanStuckLimit())
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

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("Die");
    }

    public override bool TakeDamage(float value)
    {
        animator.SetTrigger("Take Damage");
        return base.TakeDamage(value);
    }

    #endregion
}
