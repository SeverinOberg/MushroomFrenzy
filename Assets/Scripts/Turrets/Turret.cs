using UnityEngine;

public class Turret : Building 
{

    protected Animator animator;
    public TurretSO turretSO;
    public bool isSleeping = true;

    private Unit target = null;

    private Vector3 lastTargetsPosition;

    private float timeSinceLastScan;
    private float scanCooldown = 1.0f;

    private bool isWithinShootRange;
    private float distanceFromTarget;

    private float timeSinceLastShot;

    private bool flippedRight = true;

    //private float lookAtFraction = 0;
    //private float lookAtFractionMax = 3;
    //private float lookAtFractionSpeed = 3;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Health = turretSO.health;
        timeSinceLastScan = scanCooldown;
        timeSinceLastShot = turretSO.shootCooldown;
        //lastTargetsPosition = transform.position * 2.0f - transform.position;
    }

    private void Update()
    {
        if (isSleeping)
        {
            return;
        }

        timeSinceLastScan += Time.deltaTime;
        timeSinceLastShot += Time.deltaTime;

   
        if (!target)
        {
            ScanForTarget();
        }
        else
        {
            //if (!LockOnToTarget())
            //{
            //    return;
            //}

            FlipByTargetPosition();

            distanceFromTarget = Vector2.Distance(target.transform.position, transform.position);
            isWithinShootRange = distanceFromTarget <= turretSO.range;

            if (isWithinShootRange && !target.isDead)
            {
                if (IsCooldownReady(ref timeSinceLastShot, turretSO.shootCooldown))
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

    private void FlipByTargetPosition()
    {
        float xDirectionToTarget = target.transform.position.x - transform.position.x;

        if (flippedRight && xDirectionToTarget < -0.5)
        {
            flippedRight = false;
            Flip();
        }
        else if (!flippedRight && xDirectionToTarget > 0.5)
        {
            flippedRight = true;
            Flip();
        }
    }

    private void Flip()
    {
        transform.Rotate(transform.rotation.x, flippedRight ? 0.0f : 180.0f, transform.rotation.z);
    }

    private void ScanForTarget()
    {
        bool isScanReady = IsCooldownReady(ref timeSinceLastScan, scanCooldown);
        if (isScanReady)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, turretSO.range, Vector2.zero, 0.0f, LayerMask.GetMask("Enemy"));
            if (hit)
            {
                //lookAtFraction = 0;
                target = hit.transform.GetComponent<Unit>();
            }
        }
    }

    //private bool LockOnToTarget()
    //{
    //    // Lerp rotation towards the target from last target position to new target position
    //    if (lookAtFraction < lookAtFractionMax)
    //    {
    //        lookAtFraction += Time.deltaTime * lookAtFractionSpeed;
    //        Vector3 lerpPosLastAndNewTarget = Vector3.Lerp(lastTargetsPosition, target.transform.position, lookAtFraction);
    //        transform.up = lerpPosLastAndNewTarget - transform.position;
    //        return false;
    //    }
    //    else
    //    {
    //        // Lock on to target once finished lerp rotating
    //        transform.up = target.transform.position - transform.position;
    //        return true;
    //    }
    //}

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

    public virtual void Shoot()
    {
        target.TakeDamage(turretSO.damage);
        animator.SetTrigger("Shoot");
    }
}
