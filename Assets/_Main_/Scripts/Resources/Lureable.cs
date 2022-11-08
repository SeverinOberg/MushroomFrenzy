using UnityEngine;

public class Lureable : MonoBehaviour
{
    [SerializeField] float speed = 1;

    private Transform lurePoint;
    private bool isLuring;
    Vector2 direction;

    private void Update()
    {
        if (isLuring)
        {
            if (!lurePoint)
            {
                isLuring = false;
                return;
            }

            direction = (lurePoint.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
            if (Utilities.GetDistanceBetween(lurePoint.position, transform.position) < 0.1f)
            {
                Collider2D[] nearbySpiritEssence = Physics2D.OverlapCircleAll(transform.position, 0.1f, LayerMask.GetMask("Spirit Essence"));
                for (int i = 0; i < nearbySpiritEssence.Length; i++)
                {
                    if (nearbySpiritEssence[i].gameObject == gameObject)
                    {
                        continue;
                    }

                    nearbySpiritEssence[i].GetComponent<Pickup>().amount += 1;
                    Destroy(gameObject);
                    return;
                }

                enabled = false;
            }
        }
    }

    public void Initialize(Transform lurePoint)
    {
        if (isLuring) return;

        this.lurePoint = lurePoint;
        isLuring = true;
    }

}
