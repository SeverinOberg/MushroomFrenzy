using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SlimeSlinger : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private SlimeSlingerProjectile projectile;


    public override void Shoot(Unit target)
    {
        base.Shoot(target);
        shootEffect.Play();
        SlimeSlingerProjectile proj = Instantiate(projectile, transform.position, projectile.transform.rotation);
        proj.Turret = this;
    }

}
