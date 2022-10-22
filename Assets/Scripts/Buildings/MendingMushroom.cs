using System.Collections;
using UnityEngine;

public class MendingMushroom : Building 
{
    #region Variables & Properties

    [SerializeField] private float healAmount = 10f;
    [SerializeField] private Sprite sapling;
    [SerializeField] private Sprite ripe;

    private Animator animator;

    enum Stages
    {
        seed,
        sapling,
        ripe
    }
    private Stages stage;

    #endregion

    #region Unity

    protected override void Start()
    {
        base.Start();
        
        animator = GetComponent<Animator>();
        StartCoroutine("Growth");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stage != Stages.ripe)
        {
            UIManager.LogToScreen($"{unitData.title} is not ripe yet");
            return;
        }

        if (collision.transform.TryGetComponent(out PlayerController player))
        {
            if (player.Health >= player.MaxHealth)
            {
                UIManager.LogToScreen($"Can't pick up {unitData.title} while at full health");
                return;
            }

            player.TriggerAnimation("Eat");
            player.SetStopMovement(true, 0.5f);
            player.Heal(healAmount);
            Destroy(gameObject);
        }
    }

    #endregion

    #region Methods

    private IEnumerator Growth()
    {
        stage = Stages.seed;
        animator.SetTrigger("Seed");

        yield return new WaitForSeconds(5);
        stage = Stages.sapling;
        animator.SetTrigger("Sapling");

        yield return new WaitForSeconds(5);
        stage = Stages.ripe;
        animator.SetTrigger("Ripe");

        yield return new WaitForSeconds(1);
        animator.SetTrigger("Ripe Idle");
    }

    #endregion

}
