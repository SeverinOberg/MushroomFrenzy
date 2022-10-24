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
    //[SerializeField] private EnemySpawnManager    enemySpawnManager;
    [SerializeField] private Transform            spawnPoint;
    [SerializeField] private GameObject           observer;
    [SerializeField] private InfiniteWaveObject[] enemyPrefabs;
    
    [Header("Variables")]
    //[SerializeField] private float minSecondsBetweenWaves      = 45;
    //[SerializeField] private float maxSecondsBetweenWaves      = 90;
    [SerializeField] private float secondsBetweenWaves           = 45;
    [SerializeField] private float secondsBetweenSpawns          = 0.5f;
    [SerializeField] private int   currentEnemyAmountToSpawn     = 150;
    //[SerializeField] private int   maxEnemyAmountToSpawn         = 250;
    [SerializeField] private int   increaseEnemyAmountBy         = 10;
    
    private float timeUntilNextWave;
    private bool  isWaveActive;

    private bool isInfiniteModeOn;

    private void OnEnable()
    {
        UIManager.Instance.OnInfiniteModeBtnClick += TurnInfiniteModeOn;
    }

    private void OnDisable()
    {
        UIManager.Instance.OnInfiniteModeBtnClick -= TurnInfiniteModeOn;
    }

    private void Start()
    {
        timeUntilNextWave = secondsBetweenWaves;
    }

    private void Update()
    {
        if (!isInfiniteModeOn)
            return;


        timeUntilNextWave   -= Time.deltaTime;

        if (timeUntilNextWave < 1 && !isWaveActive)
        {
            isWaveActive = true;
            StartCoroutine(DoSpawnWave());
        }

        if (observer.transform.childCount <= 0 && isWaveActive)
        {
            isWaveActive = false;
            timeUntilNextWave = secondsBetweenWaves;
            currentEnemyAmountToSpawn += increaseEnemyAmountBy;

            EnemySpawnManager.OnNextLevel?.Invoke();
            EnemySpawnManager.OnWaitNextWave?.Invoke(secondsBetweenWaves);
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

    private void TurnInfiniteModeOn()
    {
        EnemySpawnManager.OnWaitNextWave?.Invoke(secondsBetweenWaves);
        isInfiniteModeOn = true;
    }

}
