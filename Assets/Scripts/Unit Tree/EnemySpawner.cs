using UnityEngine;

public class EnemySpawner : Unit 
{

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int totalMaxEnemies = 5;
    [SerializeField] private float secondsUntilSpawn = 5;

    private int enemyAmountToSpawn = 1;
    private int minEnemiesToSpawn = 1;
    private int maxEnemiesToSpawn = 2;

    private float timeSinceLastSpawn;
    [SerializeField] private float spawnCooldown = 2f;

    protected override void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (secondsUntilSpawn >= Time.time)
        {
            return;
        }

        if (timeSinceLastSpawn >= spawnCooldown)
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

}
