using UnityEngine;

public class Gate : Building 
{

    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        type = UnitTypes.Obstacle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("Open", true);
            boxCollider.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("Open", false);
            boxCollider.enabled = true;
        }
    }

}
