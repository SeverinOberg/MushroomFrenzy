//using System.Collections.Generic;
//using System.Collections;
//using UnityEngine;
//using BehaviourTree;
//using Pathfinding;

//public class EnemyBT : Unit
//{
//    public float scanDiameter       = 15;
//    public float meleeAttackRange   = 2.5f;
//    public float attackCooldown     = 2f;
//    public float minDamage          = 2;
//    public float maxDamage          = 5;

//    public Unit instigatorTarget;

//    private float meleeAttackRangeDefault;

//    //public System.Action<Unit> OnTakeDamage;
//    public System.Action OnDisableAction;

//    private void OnEnable()
//    {
//        OnSetMovementSpeed += OnMovementSpeedChangedCallback;
//    }

//    private void OnDisable()
//    {
//        OnDisableAction?.Invoke();
//        OnSetMovementSpeed -= OnMovementSpeedChangedCallback;
//    }

//    [HideInInspector] public Animator    animator;
//    [HideInInspector] public Collider2D  collision;
//    //[HideInInspector] public Rigidbody2D rb;

//    [HideInInspector] public AIDestinationSetter aiDestinationSetter;
//    [HideInInspector] public AIPath              aiPath;

//    [HideInInspector] public Unit target;

//    //protected override Node SetupTree()
//    //{
//    //    animator            = spriteRenderer.GetComponent<Animator>();
//    //    collision           = GetComponent<Collider2D>();
//    //    rb                  = GetComponent<Rigidbody2D>();

//    //    aiDestinationSetter = GetComponent<AIDestinationSetter>();
//    //    aiPath              = GetComponent<AIPath>();

//    //    type = UnitTypes.Enemy;
//    //    aiPath.maxSpeed = MovementSpeed;
//    //    meleeAttackRangeDefault = meleeAttackRange;

//    //    //new Sequence(new List<Node>
//    //    //{
//    //    //}),
//    //    //new Selector(new List<Node>
//    //    //{
//    //    //}),

//    //    // Behaviour Tree
//    //    Node root = new Selector(new List<Node>
//    //    {
//    //    // Root
//    //        // Branch [1]
//    //        new Sequence(new List<Node> 
//    //        {
//    //            new CheckAttackedByPlayer(this),
//    //            new TaskChangeTarget(this),
//    //            new Selector(new List<Node> 
//    //            {
//    //                new TaskChase(this),
//    //                new Selector(new List<Node>
//    //                {
//    //                    new CheckEnemyInMeleeRange(this),
//    //                    new TaskMeleeAttack(this),
//    //                }),
//    //            }),
//    //        }),
//    //        // Branch [1]

//    //        // Branch [2]
//    //        new Selector(new List<Node>
//    //        {
//    //            new Selector(new List<Node>
//    //            {
//    //                new CheckFOVScan(this),
//    //                new Selector(new List<Node>
//    //                {
//    //                    new TaskChase(this),
//    //                    new Selector(new List<Node>
//    //                    {
//    //                        new CheckEnemyInMeleeRange(this),
//    //                        new TaskMeleeAttack(this),
//    //                    }),
//    //                }),
//    //            }),
//    //            new TaskPatrol(this),
//    //        }),
//    //        // Branch [2]
//    //    // Root
//    //    });
//    //    // Behaviour Tree

//    //    return root;
//    //}

//    //protected override void Update()
//    //{
//    //    IsRunning();
//    //    Utilities.ForceReduceVelocity(ref rb);

//    //    // Stop AI and block Behaviour Tree Evaluations (AI updates) if IsDead
//    //    if (IsDead)
//    //    {
//    //        SetAIState(false);
//    //        return;
//    //    }

//    //    if (target)
//    //    {
//    //        FlipTowardsTarget();
//    //    }

//    //    base.Update();
//    //}

//    #region Methods

//    public bool IsWithinMeleeAttackRange()
//    {
//        if (Vector2.Distance(transform.position, target.transform.position) <= meleeAttackRange)
//        {
//            return true;
//        }
//        return false;
//    }

//    public bool IsPathPossible(Vector2 from, Vector2 to)
//    {
//        GraphNode fromNode = AstarPath.active.GetNearest(from, NNConstraint.Default).node;
//        GraphNode toNode   = AstarPath.active.GetNearest(to,   NNConstraint.Default).node;

//        if (PathUtilities.IsPathPossible(fromNode, toNode))
//        {
//            return true;
//        }

//        return false;
//    }

//    public void FlipTowardsTarget()
//    {
//        if (!aiDestinationSetter.target)
//        {
//            return;
//        }

//        if (aiDestinationSetter.target.position.x < transform.position.x)
//        {
//            spriteRenderer.flipX = true;
//        }
//        else
//        {
//            spriteRenderer.flipX = false;
//        }
//    }

//    public void ClearTarget()
//    {
//        meleeAttackRange = meleeAttackRangeDefault;
//        aiDestinationSetter.target = null;
//        target = null;
//    }

//    //public void AddForce(Vector2 direction, float forceMultiplier)
//    //{
//    //    rb.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
//    //}

//    public void SetTarget(Unit target)
//    {
//        this.target = target;
//        aiDestinationSetter.target = target.transform;
//    }

//    public void SetAIState(bool state)
//    {
//        aiDestinationSetter.enabled = state;
//        aiPath.enabled = state;
//    }

//    public void PauseAI(float pauseForSeconds = 1.0f)
//    {
//        SetAIState(false);
//        StartCoroutine(ResumeAIDelay(pauseForSeconds));
//    }

//    private IEnumerator ResumeAIDelay(float pauseForSeconds)
//    {
//        yield return new WaitForSeconds(pauseForSeconds);
//        SetAIState(true);
//    }

//    private void IsRunning()
//    {
//        if (aiPath.velocity.magnitude >= 1)
//        {
//            animator.SetFloat("Run", 1);
//        }
//        else
//        {
//            animator.SetFloat("Run", 0);
//        }
//    }

//    private void SpawnResource()
//    {
//        GameObject[] randomResources = ResourceManager.Instance.resources;
//        GameObject resource = randomResources[Random.Range(0, randomResources.Length)];
//        Instantiate(resource, transform.position, Quaternion.identity);
//    }

//    private void OnMovementSpeedChangedCallback(float value)
//    {
//        aiPath.maxSpeed = value;
//    }

//    public override bool TakeDamage(Unit instigator, float value)
//    {
//        //OnTakeDamage?.Invoke(instigator);
//        animator.SetTrigger("Take Damage");
//        return base.TakeDamage(instigator, value);
//    }

//    public override void Die(float deathDelaySeconds)
//    {
//        base.Die(deathDelaySeconds);
//        animator.SetTrigger("Die");
//        rb.simulated = false;
//        if (Utilities.Roll(33))
//        {
//            SpawnResource();
//        }
//    }

//    #endregion

//    #region Archive

//    //private float isStuckTimer;
//    //private float isStuckCooldown = 2f;
//    //private float isStuckDistanceLimit = 0.3f;
//    //private Vector2 lastStuckEvaluationPosition;

//    //public void HandleStuck(bool isPatrolling = false)
//    //{
//    //    isStuckTimer += Time.deltaTime;
//    //    if (isStuckTimer >= isStuckCooldown)
//    //    {
//    //        isStuckTimer = 0;
//    //        if (IsStuck(isPatrolling))
//    //        {
//    //            if (collision.isActiveAndEnabled)
//    //                collision.enabled = false;
//    //        }
//    //        else
//    //        {
//    //            if (!collision.isActiveAndEnabled)
//    //                collision.enabled = true;
//    //        }
//    //    }
//    //}

//    //private bool IsStuck(bool isPatrolling)
//    //{
//    //    if (isPatrolling)
//    //    {
//    //        if (HasMovedLessThanStuckLimit())
//    //        {
//    //            return true;
//    //        }
//    //    }
//    //    else
//    //    {
//    //        if (!IsWithinMeleeAttackRange() && HasMovedLessThanStuckLimit())
//    //        {
//    //            return true;
//    //        }
//    //    }

//    //    lastStuckEvaluationPosition = transform.position;
//    //    return false;
//    //}

//    //private bool HasMovedLessThanStuckLimit()
//    //{
//    //    if (Vector2.Distance(transform.position, lastStuckEvaluationPosition) <= isStuckDistanceLimit)
//    //    {
//    //        return true;
//    //    }

//    //    return false;
//    //}

//    #endregion
//}
