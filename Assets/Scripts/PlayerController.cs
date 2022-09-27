using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerController : Unit 
{

    #region Variables & Properties

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

    private bool buildMode;
    private Rigidbody2D rb;

    #endregion

    #region Unity

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        BuildingSystem.OnBuildMode += (value) => buildMode = value;
    }

    private void OnDisable()
    {
        BuildingSystem.OnBuildMode -= (value) => buildMode = value;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized;

        transform.Translate(movement * speed * Time.deltaTime);

        if (rb.velocity.normalized != Vector2.zero)
        {
            rb.velocity = rb.velocity * 0.95f;
        }

        timeSinceLastAttack += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0) && timeSinceLastAttack >= attackCooldown)
        {
            timeSinceLastAttack = 0;
            MainAttack();
        }
    }

    #endregion

    #region Methods

    private void MainAttack()
    {
        if (buildMode)
        {
            return;
        }

        Vector2 mousePos = Utilities.GetMouseWorldPosition();
        Vector2 mouseDirectionFromPlayer = (mousePos - (Vector2)transform.position).normalized;
        Vector2 offsetPos = new Vector2
        (
            transform.position.x + mouseDirectionFromPlayer.x * offsetPosMultiplier,
            transform.position.y + mouseDirectionFromPlayer.y * offsetPosMultiplier
        );

        arm.up = mouseDirectionFromPlayer;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        if (pointerEventData.selectedObject)
        {
            return;
        }

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

                if (!enemy.isDead)
                {
                    enemy.rb.AddForce(mouseDirectionFromPlayer * 10, ForceMode2D.Impulse);
                    enemy.BlinkRed();
                }
                
                enemy.PauseAI(0.2f);
            }
        }
    }

    #endregion
}
