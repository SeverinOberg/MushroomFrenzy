using UnityEngine;
using Pathfinding;
using System.Collections;

public class Merchant : Unit
{

    [Header("Merchant")]
    [SerializeField] private Animator animator;
    [SerializeField] private AIPath aiPath;
    [SerializeField] private AIDestinationSetter aiDestinationSetter;


    private string[] attackAnimationTriggers = new string[2] { "attack_1", "attack_2" };

    private float attackDamage = 10;
    private float attackSpeed  = 4;
    private bool  canAttack = true;
    
    private void Update()
    {
        HandleMovementAnimation();
        SpriteFlip();
    }

    private void HandleMovementAnimation()
    {
        if (aiPath.canMove)
        {
            animator.SetFloat("movement_speed", aiPath.velocity.magnitude);
        }
        else
        {
            animator.SetFloat("movement_speed", 0);
        }
    }

    private void SpriteFlip()
    {
        if (aiDestinationSetter.target)
        {
            Vector2 directionFromTarget = (aiDestinationSetter.target.position - transform.position).normalized;
            if (directionFromTarget.x < 0)
            {
                SpriteRenderer.flipX = true;
            }
            else
            {
                SpriteRenderer.flipX = false;
            }
        }
    }

    public override bool Attack(Unit target)
    {
        if (canAttack)
        {
            canAttack = false;
            StartCoroutine(DoResetCanAttack());

            animator.SetTrigger(attackAnimationTriggers[Random.Range(0, attackAnimationTriggers.Length)]);

            bool isDead = target.TakeDamage(attackDamage);
            target.Blink(Color.red);
            if (isDead)
            {
                return true;
            }
            
        }
        return false;
    }

    private IEnumerator DoResetCanAttack()
    {
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

}
