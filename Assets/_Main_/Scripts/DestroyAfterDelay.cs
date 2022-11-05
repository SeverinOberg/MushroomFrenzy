using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour 
{

    [SerializeField] private float timeUntilDestroy = 5;

    private void Start()
    {
        Destroy(gameObject, timeUntilDestroy);
    }

}
