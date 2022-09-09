using UnityEngine;

public class IceCannon : Turret 
{

    [SerializeField] private ParticleSystem shootEffect;

    public override void Shoot()
    {
        base.Shoot();
        //shootEffect.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(health);
            Debug.Log(movementSpeed);
            Debug.Log(unitData.title);
        }
        //Debug.Log(health);
    }

}
