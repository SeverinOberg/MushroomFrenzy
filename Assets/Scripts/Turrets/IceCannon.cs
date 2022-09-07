using UnityEngine;

public class IceCannon : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;


    public override void Shoot()
    {
        base.Shoot();
        shootEffect.Play();
    }

}
