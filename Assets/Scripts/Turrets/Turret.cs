using UnityEngine;

public class Turret : Building 
{

    [SerializeField] ParticleSystem pebbleShotEffect;

    private Unit target = null;

    private Vector3 lastTargetsPosition;

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

    private float lookAtFraction = 0;
    private float lookAtFractionMax = 3;
    private float lookAtFractionSpeed = 3;

    private void Start()
    {
        lastTargetsPosition = transform.position * 2.0f - transform.position;
        
    }

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
            if (!LockOnToTarget())
            {
                return;
            }

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
                ClearTarget(ref target);
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
                lookAtFraction = 0;
                target = hit.transform.GetComponent<Unit>();
            }
        }
    }

    private bool LockOnToTarget()
    {
        // Lerp rotation towards the target from last target position to new target position
        if (lookAtFraction < lookAtFractionMax)
        {
            lookAtFraction += Time.deltaTime * lookAtFractionSpeed;
            Vector3 lerpPosLastAndNewTarget = Vector3.Lerp(lastTargetsPosition, target.transform.position, lookAtFraction);
            transform.up = lerpPosLastAndNewTarget - transform.position;
            return false;
        }
        else
        {
            // Lock on to target once finished lerp rotating
            transform.up = target.transform.position - transform.position;
            return true;
        }
    }

    private void Shoot()
    {
        target.TakeDamage(Random.Range(minDamage, maxDamage));
        pebbleShotEffect.Play();
    }

    private void ClearTarget(ref Unit targetToClear)
    {
        lastTargetsPosition = targetToClear.transform.position;
        targetToClear = null;
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
