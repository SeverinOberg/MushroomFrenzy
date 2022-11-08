using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private Transform     resourcePopupParent;
    [SerializeField] private ResourcePopupText resourcePopupPrefab;
    [SerializeField] private float     executeDelay      = 0.15f;
    [SerializeField] private float     executeBatchDelay = 0.2f;

    private Coroutine coroutine;
    private Queue     queue = new Queue();

    public IEnumerator DoPopupQueue()
    {
        while (queue.Count > 0)
        {
            List<PopupData> data = (List<PopupData>)queue.Dequeue();
            for (int i = 0; i < data.Count; i++)
            {
                resourcePopupPrefab.Execute(data[i].amount, data[i].resourceName, data[i].isIncrease, resourcePopupParent);
                yield return new WaitForSeconds(executeBatchDelay);
            }
            
            yield return new WaitForSeconds(executeDelay);
        }
        coroutine = null;
    }

    public void Execute(List<PopupData> data)
    {
        queue.Enqueue(data);

        coroutine ??= StartCoroutine(DoPopupQueue());
    }

}
