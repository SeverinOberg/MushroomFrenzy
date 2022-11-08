using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{    
    [SerializeField] protected Rigidbody2D    rb;
    [SerializeField] protected ParticleSystem impactParticle;
    [SerializeField] private   float          shootForceMultiplier     = 25f;
    [SerializeField] private   float          knockbackForceMultiplier = 5f;
    [SerializeField] private   float          destroyDistance = 15f;

    [HideInInspector] public PlayerController instigator;
    [HideInInspector] public Vector2          direction;

    public void Spawn(PlayerController instigator, Vector2 direction)
    {
        this.instigator = instigator;
        this.direction  = direction;
        Instantiate(this, instigator.transform.position, Quaternion.identity);
    }

    private void Start()
    {
        transform.up = direction;
        rb.AddForce(direction * shootForceMultiplier, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (Vector2.Distance(instigator.transform.position, transform.position) > destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Unit hit = collision.GetComponent<Unit>();

        hit.TakeDamage(instigator.GetDamageRoll());
        hit.Blink(Color.red);
        hit.AddForce(direction, knockbackForceMultiplier);

        if (impactParticle)
            impactParticle.Play();

        if (this is ArrowProjectile)
        {
            Destroy(gameObject, 0.2f);
        }
        else
        {
            Destroy(gameObject, 0.5f);
        }
    }

}
