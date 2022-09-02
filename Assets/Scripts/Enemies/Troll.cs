using UnityEngine;

public class Troll : Enemy 
{

    new private void Start()
    {
        base.Start();
        Title = "Troll";
        Health = 15;
    }

    public override void Attack(int attackDamage)
    {
        attackDamage = Random.Range(3, 12);
        base.Attack(attackDamage);
    }
}
