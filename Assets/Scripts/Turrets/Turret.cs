using Unity.Burst.CompilerServices;
using UnityEngine;

public class Turret : Building
{

    #region Variables & Properties

    [SerializeField] protected GameObject projectilePrefab;
    public TurretSO turretData;
    public bool     isSleeping = true;

    private Unit       target = null;
    protected Animator animator;

    private bool  isWithinShootRange;
    private float distanceFromTarget;

    private float timeSinceLastScan;
    private float timeSinceLastShot;

    #endregion

    #region Unity

    protected override void Awake()
    {
        base.Awake();

        type = UnitTypes.Turret;
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        timeSinceLastScan += turretData.scanCooldown;
        timeSinceLastShot += turretData.attackSpeed;
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
            
            distanceFromTarget = Vector2.Distance(target.transform.position, transform.position);
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

    private void FlipTowardsTargetPosition()
    {
        if (transform.position.x > target.transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void ScanForTarget()
    {
        bool isScanReady = IsCooldownReady(ref timeSinceLastScan, turretData.scanCooldown);
        if (isScanReady)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, turretData.scanRange, Vector2.zero, 0.0f);
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
