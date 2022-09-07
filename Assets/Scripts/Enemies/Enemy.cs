using UnityEngine;
using Pathfinding;

public class Enemy : Unit 
{
    [SerializeField] private ParticleSystem deathParticle;

    [SerializeField] private Resource[] resources;


    protected AIPath aiPath;
    private AIDestinationSetter aiDestinationSetter;

    private float circleScanRadius = 15;
    private float distanceFromTarget;

    private float maxChaseDistance = 16;

    private float attackRange = 1.5f;
    private float attackCooldown = 2;
    private float timeSinceLastAttack;
    private bool isWithinAttackRange;

    private float scanCooldown = 1f;
    private float timeSinceLastScan;

    private Transform initialTarget;
    private Unit selectedTarget;

    private int randomResourceIndex;

    private void OnEnable()
    {
        OnMovementSpeedChanged += UpdateMovementSpeed;
    }

    private void OnDisable()
    {
        OnMovementSpeedChanged -= UpdateMovementSpeed;
    }

    protected void Start()
    {
        Faction = 1;

        aiPath = GetComponent<AIPath>();
        aiPath.maxSpeed = MovementSpeed;
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        initialTarget = GameObject.Find("Patrol Point").transform;
        aiDestinationSetter.target = initialTarget;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastScan += Time.deltaTime;

        if (selectedTarget == null)
        {
            if (timeSinceLastScan >= scanCooldown)
            {
                timeSinceLastScan = 0;
                ScanForTarget();
            }
            return;
        }

        distanceFromTarget = Vector2.Distance(transform.position, selectedTarget.transform.position);
        isWithinAttackRange = distanceFromTarget <= attackRange + selectedTarget.transform.localScale.x / 2;

        if (!isWithinAttackRange)
        {
            if (!aiPath.canMove && !isDead)
            {
                aiPath.canMove = true;
            }

            if (distanceFromTarget >= maxChaseDistance && aiDestinationSetter.target != initialTarget)
            {
                ResetTarget();
            }
        }
        else if (isWithinAttackRange)
        {
            if (aiPath.canMove)
            {
                aiPath.canMove = false;
            }

            if (timeSinceLastAttack >= attackCooldown)
            {
                timeSinceLastAttack = 0.0f;
                Attack(0);
            }
        }
    }

    private void ScanForTarget()
    {
        float closestDistanceFromHit = 0;
        Unit closestUnit = null;

        // Find closest target
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleScanRadius);
        foreach (Collider2D hit in hits)
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit == null || unit.isDead || hit.transform.CompareTag("Enemy"))
            {
                continue;
            }

            float distanceFromHit = Vector2.Distance(transform.position, unit.transform.position);

            if (closestDistanceFromHit == 0)
            {
                closestDistanceFromHit = distanceFromHit;
            }

            if (distanceFromHit <= closestDistanceFromHit)
            {
                closestDistanceFromHit = distanceFromHit;
                closestUnit = unit;
            }
        }

        // Assign new target once closest target has been found
        if (closestUnit != null)
        {
            selectedTarget = closestUnit;
            aiDestinationSetter.target = selectedTarget.transform;
        }
    }

    // Privates
    private void ResetTarget()
    {
        selectedTarget = null;
        aiDestinationSetter.target = initialTarget;
    }

    private void UpdateMovementSpeed(Unit unit, float movementSpeed)
    {
        if (unit != this.unit) { return; }
        aiPath.maxSpeed = movementSpeed;
    }

    // Virtuals
    public virtual void Attack(int attackDamage)
    {
        selectedTarget.TakeDamage(attackDamage);
    }

    
    // Overrides
    public override void Die()
    {
        base.Die();
        aiPath.canMove = false;
        Instantiate(deathParticle, transform.position, deathParticle.transform.rotation);

        randomResourceIndex = Random.Range(0, resources.Length);
        if (Random.Range(1, 101) >= 50)
        {
            Instantiate(resources[randomResourceIndex], transform.position, resources[randomResourceIndex].transform.rotation);
        }
        
    }

}