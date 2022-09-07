using System.Collections;
using UnityEngine;

public class SlimeSlinger : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private SlimeSlingerProjectile projectile;


    public override void Shoot()
    {
        base.Shoot();
        shootEffect.Play();
        SlimeSlingerProjectile proj = Instantiate(projectile, transform.position, projectile.transform.rotation);
        proj.Turret = this;
    }

}
