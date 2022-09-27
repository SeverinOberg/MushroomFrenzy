using UnityEngine;

public class IceCannon : Turret 
{

    [SerializeField] private Transform shootPoint;
    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private IceCannonProjectile projectile;

    public override void Shoot(Unit target)
    {
        base.Shoot(target);
        shootEffect.Play();
        projectile.Spawn(shootPoint.position, turretData, target);
    }

}
