using UnityEngine;

public class ForceBack : MonoBehaviour
{
    [SerializeField] private float force = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            Vector2 direction = (player.transform.position - transform.position).normalized;
            player.AddForce(direction, force);
        }
    }
}
