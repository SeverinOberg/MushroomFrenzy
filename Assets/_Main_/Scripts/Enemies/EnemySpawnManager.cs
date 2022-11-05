using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour 
{
    [System.Serializable]
    private class WaveObject
    {
        public int level;
        public WaveData[] data;
    }

    [System.Serializable]
    private class WaveData
    {
        public GameObject enemyPrefab;
        public int        spawnCount;
        public float      secondsBetweenSpawns;
    }

    [Header("Essentials")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator       animator;
    [SerializeField] private Transform      spawnPoint;
    [SerializeField] private GameObject     observer;
    [SerializeField] private WaveObject[]   waves;
    
    [Header("Variables")]
    [SerializeField] private float defaultSecondsBetweenSpawns     = 1f; // If secondsBetweenSpawns in WaveData is 0, then use this one instead.
    [SerializeField] private float secondsBetweenWaves             = 60;
    //[SerializeField] private float minSecondsBetweenWaves        = 45;
    //[SerializeField] private float maxSecondsBetweenWaves        = 90;
    //[SerializeField] private int   currentEnemyAmountToSpawn     = 50;
    //[SerializeField] private int   maxEnemyAmountToSpawn         = 250;
    //[SerializeField] private int   increaseEnemyAmountBy         = 3;

    private float secondsBetweenSpawns;
    private float timeUntilNextWave;
    private int   currentLevel = 0;
    private int   lastLevel = 0;
    private bool  isWaveActive;

    public static System.Action<float> OnWaitNextWave;
    public static System.Action OnNextLevel;

    private void Awake()
    {
        lastLevel = waves.Length;
        secondsBetweenSpawns = defaultSecondsBetweenSpawns;
        timeUntilNextWave = secondsBetweenWaves;

        OnWaitNextWave?.Invoke(secondsBetweenWaves);
    }

    private void Update()
    {
        if (GameManager.Instance.HasWon)
        {
            enabled = false;
            return;
        }

        if (currentLevel >= lastLevel)
        {
            GameManager.Instance.WinGame();
            return;
        }

        timeUntilNextWave -= Time.deltaTime;

        if (timeUntilNextWave < 1 && !isWaveActive)
        {
            isWaveActive = true;
            StartCoroutine(DoSpawnWave());
        }

        if (observer.transform.childCount <= 0 && isWaveActive)
        {
            isWaveActive = false;
            currentLevel++;
            OnNextLevel?.Invoke();
            if (currentLevel >= lastLevel)
            {
                GameManager.Instance.WinGame();
                return;
            }

            timeUntilNextWave = secondsBetweenWaves;
            OnWaitNextWave?.Invoke(secondsBetweenWaves);
        }

    }

    private IEnumerator DoSpawnWave()
    {
        for (int i = 0; i < waves[currentLevel].data.Length; i++)
        {
            if (waves[currentLevel].data[i].secondsBetweenSpawns > 0)
            {
                secondsBetweenSpawns = waves[currentLevel].data[i].secondsBetweenSpawns;
            }

            for (int j = 0; j < waves[currentLevel].data[i].spawnCount; j++)
            {
                Instantiate(waves[currentLevel].data[i].enemyPrefab, spawnPoint.position, Quaternion.identity).transform.SetParent(observer.transform, true);
                yield return new WaitForSeconds(secondsBetweenSpawns);
            }
        }

        secondsBetweenSpawns = defaultSecondsBetweenSpawns;
    }

}
