using System.Collections;
using UnityEngine;

public class SpiritMushroom : Building
{
    [SerializeField] private GameObject spiritEssencePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnDelay;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(Execute());
    }

    private IEnumerator Execute()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(spawnDelay);
            Instantiate(spiritEssencePrefab, spawnPoint.position, Quaternion.identity);
        }
    }

}
