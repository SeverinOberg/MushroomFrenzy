using UnityEngine;

public class SlimeSlingerProjectile : Projectile 
{

    [SerializeField] private GameObject goo;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyBT enemy) && !enemy.isDead)
        {
            Invoke("DestroyProjectile", turretData.slowDuration);

            animator.SetTrigger("Impact");

            // Return to default rotation, since parent rotated the goo while shooting
            goo.transform.rotation = Quaternion.identity;
            goo.SetActive(true);

            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;

            // @TODO: Consider makign this a slow damage decay instead while in the goo pool
            enemy.TakeDamage(Utilities.GetMinMaxDamageRoll(turretData.minDamage, turretData.maxDamage));

            enemy.SetMovementSpeedByPct(turretData.slowPercentage);
            enemy.SetAIState(false);
            enemy.rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyBT enemy))
        {
            enemy.MovementSpeed = enemy.unitData.movementSpeed;
            enemy.SetAIState(true);
            enemy.rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void DestroyProjectile()
    {
        // Disble simulating rigidbody to force trigger OnTriggerExit2D to resume AI to original state before destroying
        rb.simulated = false;
        Destroy(gameObject);
    }

}
