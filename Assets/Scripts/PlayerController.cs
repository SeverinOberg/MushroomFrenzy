using UnityEngine;

public class PlayerController : Unit 
{

    private float horizontalInput;
    private float verticalInput;

    private Vector2 movement;

    private float speed = 7;

    private Ray mouseDirectionRay;

    private float timeSinceLastAttack;
    private float attackCooldown = 1.0f;
    private float attackRadius = 2.0f;
    private float attackDistance = 1f;
    private float offsetPosMultiplier = 1.8f;
    private float attackDamage = 3;

    private void Start()
    {
        Title = "Mushroom Farmer";
        Health = 50;
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized;

        transform.Translate(movement * speed * Time.deltaTime);

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

        Debug.DrawRay(offsetPos, mouseDirectionFromPlayer * (attackDistance + attackRadius), Color.red, 1);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(offsetPos, attackRadius, mouseDirectionFromPlayer, attackDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Enemy") == false)
            {
                continue;
            }

            Debug.Log(hit.transform.name);
            Unit enemy = hit.transform.GetComponent<Unit>();
            if (enemy != null)
            {
                enemy.TakeDamage(Random.Range(attackDamage / 2, attackDamage * 2));
            }
        }
    }
}
