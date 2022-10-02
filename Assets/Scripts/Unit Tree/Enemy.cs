using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;

public class Enemy : Unit
{
    #region Variables / Properties

    [SerializeField] private ParticleSystem deathParticle;

    [SerializeField] private Resource[] resources;

    [HideInInspector] public Rigidbody2D rb;

    private BoxCollider2D boxCollider;
    private Animator animator;

    protected AIPath aiPath;
    private AIDestinationSetter aiDestinationSetter;

    private int waypointIndex = 0;
    private List<Transform> waypoints = new List<Transform>();

    private float circleScanRadius = 10;
    private float distanceFromTarget;

    private float maxChaseDistance = 16;

    private float attackRange = 1.5f;
    private float attackCooldown = 2;
    private float timeSinceLastAttack;
    private bool isWithinAttackRange;

    private float scanCooldown = 1f;
    private float timeSinceLastScan;

    private Unit selectedTarget;

    private int randomResourceIndex;

    #endregion

    #region Unity

    #region Subscriptions

    private void OnEnable()
    {
        OnMovementSpeedChanged += UpdateMovementSpeed;
    }

    private void OnDisable()
    {
        OnMovementSpeedChanged -= UpdateMovementSpeed;
    }

    #endregion

    #region Init

    protected override void Awake()
    {
        base.Awake();

        animator            = GetComponent<Animator>();
        rb                  = GetComponent<Rigidbody2D>();
        boxCollider         = GetComponent<BoxCollider2D>();
        aiPath              = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
    }

    protected override void Start()
    {
        base.Start();

        boxCollider.enabled = false;
        aiPath.maxSpeed = unitData.movementSpeed;

        InitWaypoints();

        aiDestinationSetter.target = waypoints[waypointIndex];
    }

    #endregion


    private float stuckScanCooldown = 3f;
    private float timeSinceLastStuckScan;
    private Vector2 lastValidationPosition;

    private void Update()
    {
        Utilities.ForceReduceVelocity(ref rb);
        
        if (isDead)
        {
            return;
        }

        if (!isWithinAttackRange)
        {
            animator.SetFloat("Run", 1);
        }

        if (aiDestinationSetter.target.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastScan   += Time.deltaTime;

        timeSinceLastStuckScan += Time.deltaTime;

        HandleReachingWaypoints();

        if (!selectedTarget)
        {
            if (timeSinceLastScan >= scanCooldown)
            {
                timeSinceLastScan = 0;
                ScanForTarget();
            }
        }
        else
        {
            distanceFromTarget = Vector2.Distance(transform.position, selectedTarget.transform.position);
            isWithinAttackRange = distanceFromTarget <= attackRange + selectedTarget.transform.localScale.x / 2;

            if (isWithinAttackRange)
            {
                animator.SetFloat("Run", 0);
            }

            if (timeSinceLastStuckScan >= stuckScanCooldown)
            {
                timeSinceLastStuckScan = 0;
                if (Vector2.Distance(lastValidationPosition, transform.position) < 1f && !isWithinAttackRange)
                {
                    float closestTargetDistance = 100;
                    Unit closestUnit = null;

                    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleScanRadius);
                    foreach (Collider2D hit in hits)
                    {
                        if (!hit.TryGetComponent(out Unit unit) || unit.isDead || hit.transform.CompareTag("Enemy"))
                        {
                            continue;
                        }

                        float targetDistance = Vector2.Distance(transform.position, unit.transform.position);

                        if (targetDistance < closestTargetDistance && unit.CompareTag("Obstacle"))
                        {
                            closestTargetDistance = targetDistance;
                            closestUnit = unit;
                        }
                    }

                    // Select the closest target found
                    if (closestUnit != null)
                    {
                        selectedTarget = closestUnit;
                        aiDestinationSetter.target = closestUnit.transform;
                    }

                    return;
                }

                lastValidationPosition = transform.position;
            }

            if (distanceFromTarget < maxChaseDistance)
            {
                boxCollider.enabled = true;
            }
            else
            {
                boxCollider.enabled = false;
            }

            if (!isWithinAttackRange)
            {
                if (distanceFromTarget >= maxChaseDistance && aiDestinationSetter.target != waypoints[waypointIndex])
                {
                    ResetTarget();
                    return;
                }
            }
            else if (isWithinAttackRange)
            {
                if (timeSinceLastAttack >= attackCooldown)
                {
                    timeSinceLastAttack = 0.0f;
                    Attack(0);
                }
            }
        }
    }

    #endregion

    #region Methods

    private void ScanForTarget()
    {
        float closestTargetDistance = 100;
        Unit closestUnit = null;

        // Find closest target
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleScanRadius);
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out Unit unit) || unit.isDead || hit.transform.CompareTag("Enemy"))
            {
                continue;
            }

            float targetDistance = Vector2.Distance(transform.position, unit.transform.position);

            if (targetDistance < closestTargetDistance && !unit.CompareTag("Obstacle"))
            {
                closestTargetDistance = targetDistance;
                closestUnit = unit;
            }
        }

        // Select the closest target found
        if (closestUnit != null)
        {
            selectedTarget = closestUnit;
            aiDestinationSetter.target = closestUnit.transform;
        }
    }

    private void ResetTarget()
    {
        selectedTarget = null;
        aiDestinationSetter.target = waypoints[waypointIndex];
    }

    private void HandleReachingWaypoints()
    {
        if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) <= 3f && waypoints.Count - 1 > waypointIndex)
        {
            waypointIndex++;
            if (aiDestinationSetter.target.gameObject == waypoints[waypointIndex - 1].gameObject)
            {
                aiDestinationSetter.target = waypoints[waypointIndex];
            }
        }
    }

    private void InitWaypoints()
    {
        Transform[] initWaypoints = GameObject.Find("Waypoints").GetComponentsInChildren<Transform>();
        for (int i = 0; i < initWaypoints.Length; i++)
        {
            // Skip the parent
            if (i == 0)
            {
                continue;
            }
            waypoints.Add(initWaypoints[i]);
        }
    }

    private void UpdateMovementSpeed(float movementSpeed)
    {
        aiPath.maxSpeed = movementSpeed;
    }

    public void AddForce(Vector2 direction, float forceMultiplier)
    {
        rb.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
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

    private void SetAIState(bool state)
    {
        aiDestinationSetter.enabled = state;
        aiPath.enabled = state;
    }

    public override void Die()
    {
        base.Die();

        aiDestinationSetter.target = transform;
        aiPath.canMove = false;
        Instantiate(deathParticle, transform.position, deathParticle.transform.rotation);
        rb.simulated = false;

        animator.SetTrigger("Die");

        randomResourceIndex = Random.Range(0, resources.Length);
        if (Random.Range(1, 101) >= 50)
        {
            Instantiate(resources[randomResourceIndex], transform.position, resources[randomResourceIndex].transform.rotation);
        }
    }

    public override void TakeDamage(float value)
    {
        base.TakeDamage(value);
        animator.SetTrigger("Take Damage");
    }

    public virtual void Attack(int attackDamage)
    {
        selectedTarget.TakeDamage(attackDamage);
        selectedTarget.BlinkRed();
        animator.SetTrigger("Attack");
    }

    #endregion

}