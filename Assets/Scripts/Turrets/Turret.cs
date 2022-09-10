using UnityEngine;

public class Turret : Building
{
    public TurretSO turretData;

    protected Animator animator;
    
    public bool isSleeping = true;

    private Unit target = null;

    private bool isWithinShootRange;
    private float distanceFromTarget;

    private float timeSinceLastScan;
    private float timeSinceLastShot;

    private bool flippedRight = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        timeSinceLastScan += turretData.scanCooldown;
        timeSinceLastShot += turretData.attackSpeed;
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
            FlipByTargetPosition();

            distanceFromTarget = Vector2.Distance(target.transform.position, transform.position);
            isWithinShootRange = distanceFromTarget <= turretData.attackRange;
            if (isWithinShootRange && !target.isDead)
            {
                if (IsCooldownReady(ref timeSinceLastShot, turretData.attackSpeed))
                {
                    Shoot(target);
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
        bool isScanReady = IsCooldownReady(ref timeSinceLastScan, turretData.scanCooldown);
        if (isScanReady)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, turretData.scanRange, Vector2.zero, 0.0f, LayerMask.GetMask("Enemy"));
            if (hit)
            {
                target = hit.transform.GetComponent<Unit>();
            }
        }
    }

    private void ClearTarget(ref Unit targetToClear)
    {
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
        //target.TakeDamage(Random.Range(turretData.minDamage, turretData.maxDamage));
        animator.SetTrigger("Shoot");
    }

    public virtual void Shoot(Unit target)
    {
        //target.TakeDamage(Random.Range(turretData.minDamage, turretData.maxDamage));
        animator.SetTrigger("Shoot");
    }
}
