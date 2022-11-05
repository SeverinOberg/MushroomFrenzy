using UnityEngine;

public class SlimeSlingerProjectile : Projectile 
{

    [SerializeField] private GameObject goo;

    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Enemy enemy) && !enemy.IsDead)
        {
            Invoke("DestroyProjectile", turretData.slowDuration);

            animator.SetTrigger("Impact");

            // Return to default rotation, since parent rotated the goo while shooting
            goo.transform.rotation = Quaternion.identity;
            goo.SetActive(true);

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;

            // @TODO: Consider making this a slow damage decay instead while in the goo pool
            enemy.TakeDamage(instigator, Utilities.GetRandomFromMinMax(turretData.minDamage, turretData.maxDamage));

            enemy.SetMovementSpeedByPct(turretData.slowPercentage);
            enemy.SetPathingActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Enemy enemy))
        {
            enemy.SetMovementSpeed(enemy.DefaultMovementSpeed);
            enemy.SetPathingActive(true);
        }
    }

    private void DestroyProjectile()
    {
        // Disble simulating rigidbody to force trigger OnTriggerExit2D to resume AI to original state before destroying
        rb.simulated = false;
        Destroy(gameObject);
    }

}
