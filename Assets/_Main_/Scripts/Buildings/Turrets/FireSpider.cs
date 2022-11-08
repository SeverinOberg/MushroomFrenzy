using System.Collections.Generic;
using UnityEngine;

public class FireSpider : Turret 
{

    [SerializeField] private GameObject shootPoint;
    [SerializeField] private ParticleSystem shootPS;

    private BoxCollider2D shootCollider;

    ContactFilter2D  contactFilter;
    List<Collider2D> colliders = new List<Collider2D>();

    protected override void Start()
    {
        base.Start();
        shootCollider = shootPoint.GetComponent<BoxCollider2D>();
        contactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
    }

    public override void Shoot(Unit target)
    {
        //shootCollider.transform.localScale = transform.localScale;
        shootPoint.transform.right = (target.transform.position - transform.position).normalized;

        shootCollider.OverlapCollider(contactFilter, colliders);
        for (int i = 0; i < colliders.Count; i++)
        {
            if (colliders[i].TryGetComponent(out Unit hit) && hit.CompareTag("Enemy"))
            {
                if (!shootPS.isPlaying)
                {
                    animator.SetTrigger("attack");
                    shootPS.Play();
                }

                hit.TakeDamage(this, Utilities.GetRandomFromMinMax(turretSO.minDamage, turretSO.maxDamage));
                hit.Blink(Color.red);
            }
        }
    }

    protected override bool FlipTowardsTargetPosition()
    {
        bool shouldFlip = base.FlipTowardsTargetPosition();
        if (shouldFlip)
        {
            shootCollider.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            shootCollider.transform.localScale = new Vector2(1, 1);
        }
        return shouldFlip;
    }
}
