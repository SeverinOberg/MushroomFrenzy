using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Unit 
{

    [SerializeField] private GameObject[] enemyPrefabs;
    // private int minEnemies = 1;
    [SerializeField] private int totalMaxEnemies = 5;
    [SerializeField] private float secondsUntilSpawn = 5;

    private int enemyAmountToSpawn = 1;
    private int minEnemiesToSpawn = 1;
    private int maxEnemiesToSpawn = 2;

    private float timeSinceLastSpawn;
    [SerializeField] private float spawnCooldown = 2f;

    private void Start()
    {
        Title = "Nest";
        Health = 30;
    }

    private void Update()
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
            Instantiate(enemyPrefab, spawnLocationOffset, enemyPrefab.transform.rotation, transform);
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }

}
