using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpider : Turret 
{

    [SerializeField] private GameObject shootPoint;
    [SerializeField] private ParticleSystem shootPS;

    private BoxCollider2D shootCollider;

    ContactFilter2D  contactFilter;
    List<Collider2D> colliders = new List<Collider2D>();

    protected override void Awake()
    {
        base.Awake();
        shootCollider = shootPoint.GetComponent<BoxCollider2D>();
        contactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
    }

    public override void Shoot(Unit target)
    {
        shootCollider.transform.localScale = transform.localScale;
        shootPoint.transform.right = (target.transform.position - transform.position).normalized;

        shootCollider.OverlapCollider(contactFilter, colliders);
        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].TryGetComponent(out Unit hit) && hit.CompareTag("Enemy"))
            {
                if (!shootPS.isPlaying)
                {
                    animator.SetTrigger("Shoot");
                    shootPS.Play();
                }

                hit.TakeDamage(Utilities.GetMinMaxDamageRoll(turretData.minDamage, turretData.maxDamage));
                hit.Blink(Color.red);
            }
        }
    }
}
