using UnityEngine;

public class Boar : Enemy 
{

    new private void Start()
    {
        base.Start();
        Title = "Boar";
        Health = 5;
    }

    public override void Attack(int attackDamage)
    {
        attackDamage = Random.Range(1, 5);
        base.Attack(attackDamage);
    }

}
