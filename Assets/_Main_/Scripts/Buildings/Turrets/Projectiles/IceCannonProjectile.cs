using UnityEngine;

public class IceCannonProjectile : Projectile 
{

    #region Variables
    [SerializeField] private GameObject body;
    [SerializeField] private float      impactRadius = 1f;

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            body.SetActive(false);
            impactParticle.Play();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, impactRadius, LayerMask.GetMask("Enemy"));
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent(out Unit unit))
                {
                    unit.Blink(Color.blue);
                    unit.TakeDamage(Utilities.GetRandomFromMinMax(turretData.minDamage, turretData.maxDamage));
                    
                    unit.SetMovementSpeedByPct(turretData.slowPercentage, turretData.slowDuration);
                }
            }
            Destroy(gameObject);
        }
    }

}
