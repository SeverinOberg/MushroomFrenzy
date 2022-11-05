using UnityEngine;

public class SetParentOnStart : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private bool worldPositionStays;

    void Start()
    {
        transform.SetParent(parent, worldPositionStays);
    }
}
