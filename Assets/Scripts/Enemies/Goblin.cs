using UnityEngine;

public class Goblin : Enemy 
{

    public override void Attack(int attackDamage)
    {
        attackDamage = Random.Range(2, 7);
        base.Attack(attackDamage);
    }

}
