using System.Collections;
using UnityEngine;

public class IceCannonProjectile : MonoBehaviour 
{

    #region Variables

    [SerializeField] private GameObject     projectile;
    [SerializeField] private ParticleSystem trailParticle;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private LineRenderer   iceTrailLine;

    private TurretSO    turretData;
    private Unit        target;

    private BoxCollider2D   boxCollider;
    private Rigidbody2D     rb;

    private bool    targetHit;
    private float   trailLimitSeconds = 3.0f;

    #endregion

    #region Unity

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();    
    }

    private void Start()
    {
        Vector3 targetDirection = (target.transform.position - transform.position).normalized;
        transform.up = targetDirection;
        rb.AddForce(targetDirection * 25, ForceMode2D.Impulse);

        iceTrailLine.SetPosition(0, transform.position);

        Destroy(gameObject, 10.0f);
        StartCoroutine(TrailHandler());
    }

    #endregion

    public void Spawn(Vector3 position, TurretSO turretData, Unit target)
    {
        var proj = Instantiate(this, position, transform.rotation);
        proj.turretData = turretData;
        proj.target = target;
    }

    private IEnumerator TrailHandler()
    {
        float timer = 0;
        while (!targetHit && timer <= trailLimitSeconds)
        {
            timer += Time.deltaTime;

            iceTrailLine.SetPosition(1, transform.position);

            boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - 0.075f);
            boxCollider.size = new Vector2(boxCollider.size.x, boxCollider.size.y + 0.15f);

            yield return new WaitForSeconds(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If projectile has already hit a target, only use slowing logic for any re-entering our collider.
        if (collision.CompareTag("Enemy") && targetHit)
        {
            Unit enemy = collision.GetComponent<Unit>();
            enemy.SetMovementSpeedByPct(turretData.slowPercentage);
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            targetHit = true;
            Invoke("DestroyProjectile", turretData.slowDuration);
            rb.bodyType = RigidbodyType2D.Static;

            Unit enemy = collision.GetComponent<Unit>();
            enemy.TakeDamage(Utilities.GetMinMaxDamageRoll(turretData.minDamage, turretData.maxDamage));
            enemy.SetMovementSpeedByPct(turretData.slowPercentage);
            enemy.BlinkRed();

            explosionParticle.Play();
            trailParticle.Stop();

            projectile.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Unit>();
            enemy.MovementSpeed = enemy.unitData.movementSpeed;
        }
    }

    private void DestroyProjectile()
    {
        rb.simulated = false;
        Destroy(gameObject);
    }

}
