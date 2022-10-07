using UnityEngine;

public class EnemySpawner : Unit 
{
    public enum NestTypes
    {
        Boar,
        Goblin,
        Troll,
    }

    public NestTypes nestType;

    [SerializeField] private GameObject[] enemyPrefabs;

    [SerializeField] private float minutesUntilSpawn             = 0f;
    [SerializeField] private float minutesUntilStronger          = 3f;
    [SerializeField] private float secondsBetweenWaves           = 10f;
    [SerializeField] private float minSecondsBetweenWaves        = 10f;
    [SerializeField] private float decreaseSecondsBetweenWavesBy = 10f;

    [SerializeField] private int currentEnemyAmountToSpawn = 5;
    [SerializeField] private int maxEnemyAmountToSpawn     = 50;
    [SerializeField] private int increaseEnemyAmountBy     = 10;

    private float   timeSinceLastSpawn;
    private Vector2 spawnLocationOffset;

    public static System.Action OnDeath;

    private Animator animator;

    #region Unity

    #region Subscriptions

    private void OnEnable()
    {
        OnDeath += OnDeathCallback;
    }

    private void OnDisable()
    {
        OnDeath -= OnDeathCallback;
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        type     = UnitTypes.Nest;

        // Convert minutes to seconds
        minutesUntilSpawn    *= 60; 
        minutesUntilStronger *= 60;
        // ---

        timeSinceLastSpawn += minSecondsBetweenWaves;

        InvokeRepeating("OnStronger", minutesUntilStronger, minutesUntilStronger);
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || minutesUntilSpawn >= Time.time)
            return;

        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= secondsBetweenWaves)
        {
            if (transform.childCount < currentEnemyAmountToSpawn * 0.5f)
            {
                timeSinceLastSpawn = 0;
                SpawnWave();
            }
        }
    }

    #endregion

    private void SpawnWave()
    {
        for (int i = 0; i < currentEnemyAmountToSpawn; i++)
        {
            spawnLocationOffset = new Vector2
            (
                transform.localPosition.x - Random.Range(-1.0f, 3.0f),
                transform.localPosition.y - Random.Range(-1.0f, 3.0f)
            );
            Instantiate(GetRandomEnemyPrefab(), spawnLocationOffset, Quaternion.identity).transform.SetParent(transform, true);
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

    private void OnStronger()
    {
        if (minutesUntilSpawn >= Time.time || currentEnemyAmountToSpawn >= maxEnemyAmountToSpawn)
            return;

        currentEnemyAmountToSpawn += increaseEnemyAmountBy;
        secondsBetweenWaves -= decreaseSecondsBetweenWavesBy;
    }

    private void OnDeathCallback()
    {
        if (currentEnemyAmountToSpawn <= maxEnemyAmountToSpawn)
            currentEnemyAmountToSpawn += increaseEnemyAmountBy;
        if (secondsBetweenWaves > minSecondsBetweenWaves)
            secondsBetweenWaves -= decreaseSecondsBetweenWavesBy;

        UIGame.LogToScreen("The enemy didn't like that, they grow stronger...");
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("Destroy");
        OnDeath?.Invoke();
    }

}
