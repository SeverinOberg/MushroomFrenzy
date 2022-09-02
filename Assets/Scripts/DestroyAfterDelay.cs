using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour 
{

    [SerializeField] private float timeUntilDestroyed = 5;

    private void Start()
    {
        Invoke("Execute", timeUntilDestroyed);
    }

    private void Execute()
    {
        Destroy(gameObject);
    }

}
