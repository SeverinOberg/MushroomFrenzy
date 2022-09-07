using UnityEngine;

public class Flamethrower : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;


    public override void Shoot()
    {
        base.Shoot();
        shootEffect.Play();
    }

}
