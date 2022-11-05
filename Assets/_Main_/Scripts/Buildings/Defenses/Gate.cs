using UnityEngine;

public class Gate : Building 
{

    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Animator      animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("Open", true);
            boxCollider.isTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("Open", false);
            boxCollider.isTrigger = false;
        }
    }

}
