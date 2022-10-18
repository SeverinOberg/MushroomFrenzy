using UnityEngine;

public class Projectile : MonoBehaviour 
{

    #region Variables / Properties

    [SerializeField] protected TurretSO       turretData;
    [SerializeField] protected ParticleSystem impactParticle;
    [SerializeField] private   float          shootForceMultiplier     = 25f;
    [SerializeField] private   float          knockbackForceMultiplier = 5f;

    protected Rigidbody2D rb;
    protected Unit        instigator;

    private Vector2 targetDirection;
    private Vector2 spawnPoint;
    private float   distanceFromSpawnPoint;
    
    #endregion

    #region Unity

    protected virtual void Awake()
    {
        rb         = GetComponent<Rigidbody2D>();
        spawnPoint = transform.position;
    }

    private void Update()
    {
        distanceFromSpawnPoint = Vector2.Distance(spawnPoint, transform.position);
        if (distanceFromSpawnPoint >= turretData.attackRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out Unit unit))
            {
                if (impactParticle)
                    impactParticle.Play();
                
                unit.TakeDamage(instigator, Utilities.GetMinMaxDamageRoll(turretData.minDamage, turretData.maxDamage));
                unit.Blink(Color.red);
                if (knockbackForceMultiplier > 0 && unit.TryGetComponent(out Enemy enemy))
                {
                    enemy.PausePathing(0.2f);
                    enemy.AddForce(targetDirection, knockbackForceMultiplier);
                }
                Destroy(gameObject, 0.5f);
            }
        }
    }

    #endregion

    #region Methods

    public void Spawn(GameObject projectile, Vector2 position, Unit instigator, Unit target)
    {
        Projectile spawn = Instantiate(projectile, position, Quaternion.identity).GetComponent<Projectile>();
        spawn.instigator = instigator;
        spawn.Shoot(target);
    }

    private void Shoot(Unit target)
    {
        targetDirection = (target.transform.position - transform.position).normalized;
        transform.up = targetDirection;
        rb.AddForce(targetDirection * shootForceMultiplier, ForceMode2D.Impulse);
    }

    #endregion

}