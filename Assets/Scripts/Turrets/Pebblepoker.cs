using UnityEngine;

public class Pebblepoker : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private GameObject projectilePrefab;

    private Projectile projectile;

    protected override void Start()
    {
        base.Start();

        projectile = projectilePrefab.GetComponent<Projectile>();
    }

    public override void Shoot(Unit target)
    {
        base.Shoot(target);

        shootEffect.Play();
        
        projectile.Spawn(projectilePrefab, transform.position, target);
    }

}
