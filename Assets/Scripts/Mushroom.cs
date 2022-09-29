using System.Collections;
using UnityEngine;

public class Mushroom : Building 
{
    #region Variables & Properties

    [SerializeField] private float amount;
    [SerializeField] private Sprite sapling;
    [SerializeField] private Sprite ripe;

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

        stage = Stages.seed;
        StartCoroutine("Growth");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stage != Stages.ripe)
        {
            // @TODO: Invoke infoText to display "Mushroom isn't quite ready for consumption"
            Debug.Log($"{unitData.title} is not ripe yet");
            return;
        }

        if (collision.transform.TryGetComponent(out PlayerController player))
        {
            if (player.health >= player.unitData.health)
            {
                // @TODO: Invoke infoText to display "Can't pick up while at full health"
                Debug.Log("Can't pick up while at full health");
                return;
            }

            player.Heal(amount);
            Destroy(gameObject);
        }
    }

    #endregion

    #region Methods

    private IEnumerator Growth()
    {
        yield return new WaitForSeconds(5);
        stage = Stages.sapling;
        spriteRenderer.sprite = sapling;

        yield return new WaitForSeconds(5);
        stage = Stages.ripe;
        spriteRenderer.sprite = ripe;
    }

    #endregion

}
