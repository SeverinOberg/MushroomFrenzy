using UnityEngine;

public class Gate : Building 
{

    [SerializeField] private BoxCollider2D boxCollider;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
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
