using System.Collections;
using UnityEngine;

public class MendingMushroom : Building 
{
    [SerializeField] private Animator animator;
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private Sprite sapling;
    [SerializeField] private Sprite ripe;

    enum Stages
    {
        seed,
        sapling,
        ripe
    }
    private Stages stage;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(Growth());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stage != Stages.ripe)
        {
            UIManager.LogToScreen($"{Title} is not ripe yet");
            return;
        }

        if (collision.transform.TryGetComponent(out PlayerController player))
        {
            if (player.Health >= player.MaxHealth)
            {
                UIManager.LogToScreen($"Can't pick up {Title} while at full health");
                return;
            }

            player.TriggerAnimation("eat");
            player.SetStopMovement(true, 0.5f);
            player.Heal(healAmount);
            Destroy(gameObject);
        }
    }

    private IEnumerator Growth()
    {
        stage = Stages.seed;
        animator.SetTrigger("seed");

        yield return new WaitForSeconds(5);
        stage = Stages.sapling;
        animator.SetTrigger("sapling");

        yield return new WaitForSeconds(5);
        stage = Stages.ripe;
        animator.SetTrigger("ripe");

        yield return new WaitForSeconds(1);
        animator.SetTrigger("ripe_idle");
    }

}
