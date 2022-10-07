using System.Collections.Generic;
using UnityEngine;

public class FireSpider : Turret 
{

    [SerializeField] private GameObject shootObject;
    [SerializeField] private ParticleSystem shootPS;

    ParticleSystem.MainModule shootPSMain;
    float initialPSMainRot;

    private BoxCollider2D shootCollider;

    private float timeSinceLastBurn;

    ContactFilter2D contactFilter;
    List<Collider2D> colliders = new List<Collider2D>();

    protected override void Awake()
    {
        base.Awake();
        shootCollider = shootObject.GetComponent<BoxCollider2D>();
        shootPSMain = shootPS.main;
        initialPSMainRot = shootPSMain.startRotation.constant;

        contactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
    }

    protected override void Update()
    {
        if (target && target.isDead)
            return;

        base.Update();

        if (!target)
        {
            return;
        }
        
        timeSinceLastBurn += Time.deltaTime;
        if (timeSinceLastBurn >= turretData.attackSpeed)
        {
            shootCollider.transform.localScale = transform.localScale;
            shootObject.transform.right = (target.transform.position - transform.position).normalized;

            shootCollider.OverlapCollider(contactFilter, colliders);
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].TryGetComponent(out Unit unit) && unit.CompareTag("Enemy"))
                {
                    if (!shootPS.isPlaying)
                    {
                        animator.SetTrigger("Shoot");
                        shootPS.Play();
                    }
                    
                    unit.TakeDamage(Utilities.GetMinMaxDamageRoll(turretData.minDamage, turretData.maxDamage));
                    unit.BlinkRed();
                }
            }
        }

    }

    public override void Shoot(Unit target)
    {

    }

}
