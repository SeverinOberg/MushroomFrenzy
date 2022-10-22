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
                gameObject.layer = 0;
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
