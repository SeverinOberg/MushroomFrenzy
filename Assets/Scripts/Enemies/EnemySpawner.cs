using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Unit 
{

    [SerializeField] private GameObject[] enemyPrefabs;
    // private int minEnemies = 1;
    private int totalMaxEnemies = 5;

    private int enemyAmountToSpawn = 1;
    private int minEnemiesToSpawn = 1;
    private int maxEnemiesToSpawn = 3;

    private float timeSinceLastSpawn;
    private float spawnCooldown = 2f;
    private float minSpawnCooldown = 5f;
    private float maxSpawnCooldown = 10f;

    private void Start()
    {
        Title = "Nest";
        Health = 30;
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnCooldown)
        {
            if (transform.childCount < totalMaxEnemies)
            {
                timeSinceLastSpawn = 0;
                spawnCooldown = Random.Range(minSpawnCooldown, maxSpawnCooldown);

                enemyAmountToSpawn = Random.Range(minEnemiesToSpawn, maxEnemiesToSpawn);
                SpawnEnemy(enemyAmountToSpawn);
            }
        }
    }

    private void SpawnEnemy(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            float randomOffsetX = Random.Range(-2, -4);
            float randomOffsetY = Random.Range(-2, -4);
            Vector2 spawnLocationOffset = new Vector2
            (
                transform.localPosition.x - randomOffsetX,
                transform.localPosition.y - randomOffsetY
            );

            GameObject spawn = Instantiate(GetRandomEnemyPrefab(), spawnLocationOffset, transform.rotation, transform);
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

}
