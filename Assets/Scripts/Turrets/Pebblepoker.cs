using UnityEngine;

public class Pebblepoker : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private PebblepokerProjectile projectile;


    public override void Shoot(Unit target)
    {
        base.Shoot(target);

        shootEffect.Play();
        projectile.Spawn(projectile, transform.position, transform, turretData, target);
    }

}
