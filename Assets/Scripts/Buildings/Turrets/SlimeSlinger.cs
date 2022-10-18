using System.Collections;
using UnityEngine;

public class SlimeSlinger : Turret 
{

    [SerializeField] private SlimeSlingerProjectile projectile;

    protected override void Start()
    {
        base.Start();
        SetLoaded(true);
    }

    public override void Shoot(Unit target)
    {
        base.Shoot(target);
        StartCoroutine(ShootRoutine(target));
    }

    private IEnumerator ShootRoutine(Unit target)
    {
        yield return new WaitForSeconds(0.3f);
        
        projectile.Spawn(projectilePrefab, transform.position, this, target);

        SetLoaded(false);
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(2);
        animator.SetBool("Loaded", true);
    }

    private void SetLoaded(bool value)
    {
        animator.SetBool("Loaded", value);
    }



}
