using UnityEngine;

public class EnemySpawner : Unit 
{

    [SerializeField] private GameObject[] enemyPrefabs;

    [Tooltip("The maximum number of enemies allowed to exist at one time from this spawner")]
    [SerializeField] private int totalMaxEnemies = 5;
    [Tooltip("Will start spawning enemies after the minutes set has passed")]
    [SerializeField] private float minutesUntilSpawn = 0.5f;
    [Tooltip("Time between spawn per enemy")]
    [SerializeField] private float secondsBetweenSpawns = 2f;
    
    private Animator animator;

    private int enemyAmountToSpawn = 1;
    private int minEnemiesToSpawn  = 1;
    private int maxEnemiesToSpawn  = 2;

    private float timeSinceLastSpawn;

    protected override void Awake()
    {
        base.Awake();
        type = UnitTypes.Nest;
        animator = GetComponent<Animator>();

        // Convert minutes to seconds for code usage
        minutesUntilSpawn *= 60;
    }

    protected override void Update()
    {
        if (isDead)
        {
            return;
        }

        timeSinceLastSpawn += Time.deltaTime;

        if (minutesUntilSpawn >= Time.time)
        {
            return;
        }

        if (timeSinceLastSpawn >= secondsBetweenSpawns)
        {
            if (transform.childCount < totalMaxEnemies)
            {
                timeSinceLastSpawn = 0;

                enemyAmountToSpawn = Random.Range(minEnemiesToSpawn, maxEnemiesToSpawn);
                SpawnEnemy(enemyAmountToSpawn);
            }
        }
        base.Update();
    }

    private void SpawnEnemy(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            float randomOffsetX = Random.Range(-1.0f, 3.0f);
            float randomOffsetY = Random.Range(-1.0f, 3.0f);
            Vector2 spawnLocationOffset = new Vector2
            (
                transform.localPosition.x - randomOffsetX,
                transform.localPosition.y - randomOffsetY
            );
            GameObject enemyPrefab = GetRandomEnemyPrefab();
            var e = Instantiate(enemyPrefab, spawnLocationOffset, enemyPrefab.transform.rotation);
            e.transform.SetParent(transform, true);
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("Destroy");
    }

}
