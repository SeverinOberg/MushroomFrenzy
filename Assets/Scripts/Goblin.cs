using UnityEngine;

public class Goblin : Enemy 
{

    new private void Start()
    {
        base.Start();
        Title = "Goblin";
        Health = 8;
    }

    public override void Attack(int attackDamage)
    {
        attackDamage = Random.Range(2, 7);
        base.Attack(attackDamage);
    }

}
