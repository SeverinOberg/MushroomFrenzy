using UnityEngine;

public class Pebblepoker : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;


    public override void Shoot()
    {
        base.Shoot();
        shootEffect.Play();
    }

}
