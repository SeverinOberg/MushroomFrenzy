using UnityEngine;
using Pathfinding;
using System.Collections;
using System.IO;

public class Enemy : Unit 
{
    private AIPath aiPath;
    private AIDestinationSetter aiDestinationSetter;

    private float circleScanRadius = 15;
    private float distanceFromTarget;

    private float maxChaseDistance = 15;

    private float attackRange = 1.5f;
    private float attackCooldown = 2;
    private float timeSinceLastAttack;
    private bool isWithinAttackRange;

    private float scanCooldown = 1;
    private float timeSinceLastScan;

    private Transform initialTarget;
    private Unit selectedTarget;

    protected void Start()
    {
        Faction = 1;

        aiPath = GetComponent<AIPath>();
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        initialTarget = FindObjectOfType<Farm>().transform;
        aiDestinationSetter.target = initialTarget;
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastScan += Time.deltaTime;

        distanceFromTarget = Vector2.Distance(transform.position, aiDestinationSetter.target.position);
        isWithinAttackRange = distanceFromTarget <= attackRange;

        if (selectedTarget != null && selectedTarget.isDead)
        {
            ResetTarget();
        }
        
        if (selectedTarget == null && timeSinceLastScan >= scanCooldown)
        {
            timeSinceLastScan = 0;
            ScanForTarget();
        }

        if (selectedTarget != null && !isWithinAttackRange)
        {
            if (!aiPath.canMove)
            {
                aiPath.canMove = true;
            }
            Chase();
        }
        else if (selectedTarget != null && isWithinAttackRange)
        {
            if (aiPath.canMove)
            {
                aiPath.canMove = false;
            }

            if (timeSinceLastAttack >= attackCooldown)
            {
                timeSinceLastAttack = 0.0f;
                Attack(1);
            }
        }
    }

    private void ScanForTarget()
    {
        float distanceFromHit = 0;
        float closestDistanceFromHit = 0;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, circleScanRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                return;
            }

            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit != null && !unit.isDead)
            {
                distanceFromHit = Vector2.Distance(transform.position, unit.transform.position);

                if (distanceFromHit <= closestDistanceFromHit || closestDistanceFromHit == 0)
                {
                    closestDistanceFromHit = distanceFromHit;
                    selectedTarget = unit;
                }
            }
        }
    }

    private void Chase()
    {
        if (aiDestinationSetter.target != selectedTarget.transform)
        {
            aiDestinationSetter.target = selectedTarget.transform;
        }
        else if (distanceFromTarget >= maxChaseDistance && aiDestinationSetter.target == selectedTarget.transform)
        {
            ResetTarget();
        }
    }

    private void ResetTarget()
    {
        aiPath.canMove = true;
        aiDestinationSetter.target = initialTarget;
        selectedTarget = null;
    }

    public virtual void Attack(int attackDamage)
    {
        selectedTarget.TakeDamage(attackDamage);
    }

}
