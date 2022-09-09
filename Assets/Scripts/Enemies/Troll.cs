using UnityEngine;

public class Troll : Enemy 
{
    public override void Attack(int attackDamage)
    {
        attackDamage = Random.Range(3, 12);
        base.Attack(attackDamage);
    }
}
