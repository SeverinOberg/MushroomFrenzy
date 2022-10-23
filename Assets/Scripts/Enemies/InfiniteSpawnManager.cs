using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class InfiniteSpawnManager : MonoBehaviour
{
    [System.Serializable]
    private class InfiniteWaveObject
    {
        public GameObject enemyPrefab;
        public int        spawnChance;
    }

    [Header("Essentials")]
    [SerializeField] private Transform            spawnPoint;
    [SerializeField] private GameObject           observer;
    [SerializeField] private InfiniteWaveObject[] enemyPrefabs;
    
    [Header("Variables")]
    //[SerializeField] private float minSecondsBetweenWaves      = 45;
    //[SerializeField] private float maxSecondsBetweenWaves      = 90;
    [SerializeField] private float secondsBetweenWaves           = 45;
    [SerializeField] private float secondsBetweenSpawns          = 0.5f;
    [SerializeField] private int   currentEnemyAmountToSpawn     = 150;
    [SerializeField] private int   maxEnemyAmountToSpawn         = 250;
    [SerializeField] private int   increaseEnemyAmountBy         = 10;
    

    private float timeUntilNextUpdate;
    private float updateCooldown = 1;
    
    private float timeUntilNextWave;
    private bool  isWaveActive;

    private void Start()
    {
        timeUntilNextWave = secondsBetweenWaves;
    }

    private void Update()
    {
        timeUntilNextWave   -= Time.deltaTime;
        timeUntilNextUpdate -= Time.deltaTime;

        if (timeUntilNextWave < 1 && !isWaveActive)
        {
            isWaveActive = true;
            StartCoroutine(DoSpawnWave());
        }

        if (observer.transform.childCount <= 0 && isWaveActive)
        {
            isWaveActive = false;
            timeUntilNextWave = secondsBetweenWaves;

            if (currentEnemyAmountToSpawn >= maxEnemyAmountToSpawn)
            {
                return;
            }

            currentEnemyAmountToSpawn += increaseEnemyAmountBy;
        }

        if (timeUntilNextUpdate <= 0 && !isWaveActive)
        {
            timeUntilNextUpdate = updateCooldown;
            Debug.Log((int)timeUntilNextWave);
        }
    }

    private IEnumerator DoSpawnWave()
    {
        for (int i = 0; i < currentEnemyAmountToSpawn; i++)
        {
            Instantiate(GetRandomEnemyPrefab(), spawnPoint.position, Quaternion.identity).transform.SetParent(observer.transform, true);
            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        int randomPercent = Random.Range(1, 101);

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i].spawnChance >= randomPercent)
            {
                return enemyPrefabs[i].enemyPrefab;
            }
        }

        return null;
    }

}
