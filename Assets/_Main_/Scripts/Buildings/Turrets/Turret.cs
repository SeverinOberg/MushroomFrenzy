using UnityEngine;

public class Turret : Building
{

    public TurretSO turretSO;
    [SerializeField] protected GameObject projectilePrefab;

    public bool isSleeping = true;

    protected Unit     target = null;
    protected Animator animator;

    protected bool isWithinShootRange;
    private float  distanceFromTarget;

    private float timeSinceLastScan;
    private float timeSinceLastShot;

    private LayerMask enemyMask;

    protected override void Start()
    {
        base.Start();
        animator = SpriteRenderer.GetComponent<Animator>();

        enemyMask = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        if (isSleeping || IsDead)
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
            
            distanceFromTarget = Vector2.Distance(transform.position, target.transform.position);
            isWithinShootRange = distanceFromTarget <= turretSO.attackRange;
            if (isWithinShootRange && !target.IsDead)
            {
                FlipTowardsTargetPosition();

                if (IsCooldownReady(ref timeSinceLastShot, turretSO.attackSpeed))
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

    protected virtual bool FlipTowardsTargetPosition()
    {
        bool shouldFlip = transform.position.x > target.transform.position.x;
        if (shouldFlip)
        {
            SpriteRenderer.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            SpriteRenderer.transform.localScale = new Vector2(1, 1);
        }
        return shouldFlip;
    }

    private void ScanForTarget()
    {
        if (IsCooldownReady(ref timeSinceLastScan, turretSO.scanCooldown))
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, turretSO.scanRange, Vector2.zero, 0.0f, enemyMask);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.CompareTag("Enemy"))
                {
                    target = hits[i].transform.GetComponent<Unit>();
                    return;
                }
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

    public override void Die(float deathDelaySeconds)
    {
        base.Die(deathDelaySeconds);
        animator.SetTrigger("die");
    }

    public virtual void Shoot(Unit target)
    {
        animator.SetTrigger("attack");
    }

}
