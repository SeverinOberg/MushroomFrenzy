using System;
using UnityEngine;

public class Projectile : MonoBehaviour 
{

    #region Variables / Properties

    [SerializeField] protected TurretSO       turretData;
    [SerializeField] private ParticleSystem impactParticle;
    [SerializeField] private float shootForceMultiplier = 25f;
    [SerializeField] private float knockbackForceMultiplier = 5f;

    protected Rigidbody2D rb;

    private Vector2 targetDirection;
    private Vector2 spawnPoint;
    private float   distanceFromSpawnPoint;
    
    #endregion

    #region Unity

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
                impactParticle.Play();
                unit.TakeDamage(Utilities.GetMinMaxDamageRoll(turretData.minDamage, turretData.maxDamage));
                unit.BlinkRed();
                if (unit.TryGetComponent(out EnemyBT enemy))
                {
                    enemy.PauseAI(0.2f);
                    enemy.AddForce(targetDirection, knockbackForceMultiplier);
                }
                Destroy(gameObject, 0.5f);
            }
        }
    }

    #endregion

    #region Methods

    public void Spawn(GameObject projectile, Vector2 position, Unit target)
    {
        Projectile spawn = Instantiate(projectile, position, Quaternion.identity).GetComponent<Projectile>();
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
