using System;
using UnityEngine;

public class Turret : Building
{

    #region Variables & Properties

    public TurretSO turretData;
    [SerializeField] protected GameObject projectilePrefab;

    public bool        isSleeping = true;

    protected Unit     target = null;
    protected Animator animator;

    protected bool isWithinShootRange;
    private float  distanceFromTarget;

    private float timeSinceLastScan;
    private float timeSinceLastShot;

    private LayerMask enemyMask;

    #endregion

    #region Unity

    protected override void Awake()
    {
        base.Awake();
        animator = spriteRenderer.GetComponent<Animator>();

        type = UnitTypes.Turret;
        enemyMask = LayerMask.GetMask("Enemy");
    }

    protected override void Update()
    {
        if (isSleeping || isDead)
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
            isWithinShootRange = distanceFromTarget <= turretData.attackRange;
            if (isWithinShootRange && !target.isDead)
            {
                FlipTowardsTargetPosition();

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

        base.Update();
    }

    #endregion

    #region Methods

    protected virtual bool FlipTowardsTargetPosition()
    {
        bool shouldFlip = transform.position.x > target.transform.position.x;
        if (shouldFlip)
        {
            spriteRenderer.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            spriteRenderer.transform.localScale = new Vector2(1, 1);
        }
        return shouldFlip;
    }

    private void ScanForTarget()
    {
        if (IsCooldownReady(ref timeSinceLastScan, turretData.scanCooldown))
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, turretData.scanRange, Vector2.zero, 0.0f, enemyMask);
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

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("Destroy");
    }

    public virtual void Shoot(Unit target)
    {
        animator.SetTrigger("Shoot");
    }

    #endregion
}
