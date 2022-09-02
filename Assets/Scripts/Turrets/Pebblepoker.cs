using UnityEngine;

public class Pebblepoker : Turret 
{

    private Unit target = null;
    private Unit lastTarget = null;

    private float timeSinceLastScan;
    private float scanCooldown = 1.0f;
    private float scanRadius = 15.0f;

    private bool isWithinShootRange;
    private float distanceFromTarget;
    private float maxShootRange = 15.0f;

    private float timeSinceLastShot;
    private float shootCooldown = 2;

    private float minDamage = 1;
    private float maxDamage = 2;

    private void Update()
    {
        timeSinceLastScan += Time.deltaTime;
        timeSinceLastShot += Time.deltaTime;

        if (!target)
        {
            ScanForTarget();
        }
        else
        {
            //if (lastTarget != target)
            //{
            //    // rotate slowly through towards target through lerp of last and new target

            //    lastTarget = target;
            //}
            //else
            //{

            //}
            transform.up = target.transform.position - transform.position;


            distanceFromTarget = Vector2.Distance(target.transform.position, transform.position);
            isWithinShootRange = distanceFromTarget <= maxShootRange;

            if (isWithinShootRange && !target.isDead)
            {
                if (IsCooldownReady(ref timeSinceLastShot, shootCooldown))
                {
                    Shoot();
                }
            }
            else
            {
                ResetTarget();
            }
        }
    }

    private void ScanForTarget()
    {
        bool isScanReady = IsCooldownReady(ref timeSinceLastScan, scanCooldown);
        if (isScanReady)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, scanRadius, Vector2.zero, 0.0f, LayerMask.GetMask("Enemy"));
            if (hit)
            {
                target = hit.transform.GetComponent<Unit>();
            }
        }
    }

    private void Shoot()
    {
        target.TakeDamage(Random.Range(minDamage, maxDamage));
    }

    private void ResetTarget()
    {
        target = null;
    }

    private bool IsCooldownReady(ref float timeSinceLastAction, float cooldown)
    {
        if (timeSinceLastAction >= cooldown)
        {
            timeSinceLastAction = 0;
            return true;
        }
        else 
        { 
            return false; 
        }
    }
}
