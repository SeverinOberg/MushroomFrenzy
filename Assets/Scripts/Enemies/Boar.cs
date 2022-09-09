using UnityEngine;

public class Boar : Enemy 
{

    public override void Attack(int attackDamage)
    {
        attackDamage = Random.Range(1, 5);
        base.Attack(attackDamage);
    }

}
