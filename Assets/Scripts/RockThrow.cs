using UnityEditor.UI;
using UnityEngine;

public class RockThrow : MonoBehaviour
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

                target.TakeDamage(Utilities.GetMinMaxDamageRoll(instigator.MinDamage, instigator.MaxDamage));
                target.Blink(Color.red);
                target.AddForce(targetDirection, knockbackForceMultiplier);
                
                Destroy(gameObject, 0.15f);
            }
        }
    }

    public void Spawn(GameObject prefab, Vector2 position, Enemy instigator, Unit target)
    {
        RockThrow spawn = Instantiate(prefab, position, Quaternion.identity).GetComponent<RockThrow>();
        spawn.instigator = instigator;
        spawn.Throw(target);
    }

    private void Throw(Unit target)
    {
        targetDirection = (target.transform.position * instigator.GetRangedAccuracyOffset() - transform.position).normalized;

        //targetDirection = new Vector2(targetDirection.x + instigator.GetRangedAccuracyOffset(), targetDirection.x + instigator.GetRangedAccuracyOffset());

        #region Archive
        //// Throw from above and make it fall. But has to be flipped with negative gravity if the target is +y from instigator
        //Vector2 offsetDirectionAngleUpwards;
        //if (Utilities.Roll(50))
        //{
        //    offsetDirectionAngleUpwards = new Vector2(targetDirection.x * 0.5f, 1);
        //}
        //else
        //{
        //    offsetDirectionAngleUpwards = new Vector2(-targetDirection.x * 0.5f, 1);
        //}
        #endregion

        rb.AddForce(targetDirection * shootForceMultiplier, ForceMode2D.Force);
        Invoke("DoEnableCollision", 0.3f);
    }

    private void DoEnableCollision()
    {
        collision.enabled = true;   
    }

}
