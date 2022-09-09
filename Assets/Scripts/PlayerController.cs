using Pathfinding;
using System.Collections;
using UnityEngine;

public class PlayerController : Unit 
{
    [SerializeField] Animator attackAnimator;

    [SerializeField] Transform arm;

    private float horizontalInput;
    private float verticalInput;

    private Vector2 movement;

    private float speed = 7;

    private float timeSinceLastAttack;
    private float attackCooldown = 1.0f;
    private float attackRadius = 1.8f;
    private float attackDistance = 0.5f;
    private float offsetPosMultiplier = 1.8f;
    private float attackDamage = 3;

    private Enemy currentEnemy;

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized;

        transform.Translate(movement * speed * Time.deltaTime);

        timeSinceLastAttack += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0) && timeSinceLastAttack >= attackCooldown)
        {
            timeSinceLastAttack = 0;
            MainAttack();
        }
    }
    
    private void MainAttack()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDirectionFromPlayer = (mousePos - (Vector2)transform.position).normalized;
        Vector2 offsetPos = new Vector2
        (
            transform.position.x + mouseDirectionFromPlayer.x * offsetPosMultiplier,
            transform.position.y + mouseDirectionFromPlayer.y * offsetPosMultiplier
        );

        arm.up = mouseDirectionFromPlayer;
        attackAnimator.SetTrigger("Attack");

        Debug.DrawRay(offsetPos, mouseDirectionFromPlayer * (attackDistance + attackRadius), Color.red, 1);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(offsetPos, attackRadius, mouseDirectionFromPlayer, attackDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Enemy") == false)
            {
                continue;
            }

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Random.Range(attackDamage / 2, attackDamage * 2));
                enemy.rb.AddForce(mouseDirectionFromPlayer * 10, ForceMode2D.Impulse);

                if (!enemy.isDead)
                {
                    enemy.BlinkRed();
                }
                
                enemy.PauseAI(0.2f);
            }
        }
    }
}
