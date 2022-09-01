using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Unit 
{

    [SerializeField] private GameObject[] enemyPrefabs;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    // private int minEnemies = 1;
    private int maxEnemies = 5;
    private int enemyAmountToSpawn = 1;
    private int minEnemiesToSpawn = 0;
    private int maxEnemiesToSpawn = 5;

    private float timeSinceLastSpawn;
    private float spawnCooldown = 2f;
    private float minSpawnCooldown = 2f;
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
            if (spawnedEnemies.Count < maxEnemies)
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
        for (int i = 0; i <= spawnAmount; i++)
        {
            Vector2 spawnLocationOffset = new Vector2
            (
                transform.localPosition.x - 2,
                transform.localPosition.y - 2
            );

            GameObject spawn = Instantiate(GetRandomEnemyPrefab(), spawnLocationOffset, transform.rotation, transform);
            spawnedEnemies.Add(spawn);
        }
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    }



}
