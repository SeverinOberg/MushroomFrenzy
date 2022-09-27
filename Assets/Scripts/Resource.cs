using UnityEngine;

public class Resource : MonoBehaviour 
{

    private enum TypeOfResource {wood, stone};

    [SerializeField] private TypeOfResource typeOfResource;
    [SerializeField] private int amount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (typeOfResource == TypeOfResource.wood)
            {
                ResourceManager.Instance.Wood += amount;
            }
            else
            {
                ResourceManager.Instance.Stone += amount;
            }

            Destroy(gameObject);
        }
    }

}
