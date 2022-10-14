using UnityEngine;

public class Pebblepoker : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;
    private ParticleSystem.ShapeModule shootEffectShapeModule;

    private Projectile projectile;

    protected override void Start()
    {
        base.Start();

        shootEffectShapeModule = shootEffect.shape;
        projectile = projectilePrefab.GetComponent<Projectile>();
    }

    public override void Shoot(Unit target)
    {
        base.Shoot(target);

        shootEffect.Play();
        
        projectile.Spawn(projectilePrefab, transform.position, this, target);
    }

    protected override bool FlipTowardsTargetPosition()
    {
        bool shouldFlip = base.FlipTowardsTargetPosition();
        if (shouldFlip)
        {
            shootEffect.transform.rotation = new Quaternion(0, 180, -30, 0);
        }
        else
        {
            shootEffect.transform.rotation = new Quaternion(0, 0, -30, 0);
        }
        return shouldFlip;
    }

}
