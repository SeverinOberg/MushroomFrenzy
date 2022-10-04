using UnityEngine;

public class SlimeSlingerProjectile : MonoBehaviour 
{

    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject goo;

    private Rigidbody2D rb;

    private Turret turret;

    public Turret Turret
    {
        get { return turret; }
        set
        {
            if (!turret)
            {
                turret = value;
            }
            else
            {
                Debug.LogWarning("The Projectile's turret will only be set on initilization. You can no set it again.");
            }
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(turret.transform.up * 25, ForceMode2D.Impulse);
        Destroy(gameObject, 15);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out EnemyBT enemy) && !enemy.isDead)
        {
            Invoke("DestroyProjectile", turret.turretData.slowDuration);

            projectile.SetActive(false);
            goo.SetActive(true);

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;

            enemy.SetMovementSpeedByPct(turret.turretData.slowPercentage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out EnemyBT enemy))
        {
            enemy.MovementSpeed = enemy.unitData.movementSpeed;
        }
    }

    private void DestroyProjectile()
    {
        rb.simulated = false;
        Destroy(gameObject);
    }

}
