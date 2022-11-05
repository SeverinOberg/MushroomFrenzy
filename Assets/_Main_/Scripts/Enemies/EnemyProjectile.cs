using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    
    [SerializeField] private ParticleSystem impactParticle;
    [SerializeField] private float          shootForceMultiplier     = 500f;
    [SerializeField] private float          knockbackForceMultiplier = 5f;

    private Collider2D  collision;
    private Rigidbody2D rb;
    private Enemy       instigator;

    private Vector2     targetDirection;
    private Vector2     spawnPoint;
    private float       distanceFromSpawnPoint;

    private void Awake()
    {
        collision  = GetComponent<Collider2D>();
        rb         = GetComponent<Rigidbody2D>();

        spawnPoint = transform.position;
    }

    private void Update()
    {
        distanceFromSpawnPoint = Vector2.Distance(spawnPoint, transform.position);
        if (distanceFromSpawnPoint >= instigator.RangedAttackRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.transform.CompareTag("Enemy") && !collision.isTrigger)
        {
            if (collision.TryGetComponent(out Unit target))
            {
                if (impactParticle)
                    impactParticle.Play();

                target.TakeDamage(Utilities.GetRandomFromMinMax(instigator.MinMeleeDamage, instigator.MaxMeleeDamage));
                target.Blink(Color.red);
                target.AddForce(targetDirection, knockbackForceMultiplier);
                
                Destroy(gameObject, 0.15f);
            }
        }
    }

    public void Spawn(GameObject prefab, Vector2 position, Enemy instigator, Unit target)
    {
        EnemyProjectile spawn = Instantiate(prefab, position, Quaternion.identity).GetComponent<EnemyProjectile>();
        spawn.instigator = instigator;
        spawn.Throw(target);
    }

    private void Throw(Unit target)
    {
        if (!target)
            return;

        targetDirection = (target.transform.position - transform.position).normalized;
        transform.up = targetDirection;
        rb.AddForce(targetDirection * shootForceMultiplier, ForceMode2D.Force);
        Invoke("DoEnableCollision", 0.3f);
    }

    private void DoEnableCollision()
    {
        collision.enabled = true;   
    }
}
