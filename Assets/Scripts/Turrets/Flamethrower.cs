using UnityEngine;

public class Flamethrower : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;


    public override void Shoot(Unit target)
    {
        base.Shoot(target);
        shootEffect.Play();
        target.TakeDamage(Utilities.GetMinMaxDamageRoll(turretData.minDamage, turretData.maxDamage));
    }

}
