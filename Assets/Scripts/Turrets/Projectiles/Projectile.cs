using UnityEngine;

public class Projectile : MonoBehaviour 
{

    #region Variables / Properties

    [SerializeField] private ParticleSystem impactParticle;
    [SerializeField] private float shootForceMultiplier = 25f;
    [SerializeField] private float knockbackForceMultiplier = 5f;

    protected Rigidbody2D rb;
    protected TurretSO    turretData;
    protected Unit        target;

    private Vector2 targetDirection;
    private Vector2 spawnPoint;
    private float   distanceFromSpawnPoint;
    

    #endregion

    #region Unity

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        spawnPoint = transform.position;

        targetDirection = (target.transform.position - transform.position).normalized;
        transform.up = targetDirection;
        rb.AddForce(targetDirection * shootForceMultiplier, ForceMode2D.Impulse);
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
            if (collision.TryGetComponent(out Enemy enemy))
            {
                impactParticle.Play();
                enemy.TakeDamage(Utilities.GetMinMaxDamageRoll(turretData.minDamage, turretData.maxDamage));
                enemy.BlinkRed();
                enemy.PauseAI(0.2f);
                enemy.AddForce(targetDirection, knockbackForceMultiplier);
            }
        }
    }

    #endregion

    #region Methods

    public void Spawn(Projectile projectile, Vector2 position, Transform parent, TurretSO turretData, Unit target)
    {
        var proj = Instantiate(projectile, position, Quaternion.identity, parent);
        proj.turretData = turretData;
        proj.target     = target;
    }

    #endregion

}
