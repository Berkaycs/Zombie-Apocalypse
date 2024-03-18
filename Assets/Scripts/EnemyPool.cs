using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    private WaitForSeconds _spawnDelay = new WaitForSeconds(3);

    [Serializable]
    public struct Pool
    {
        public Queue<GameObject> PooledEnemies;
        public GameObject EnemyPrefab;
        public int PoolSize;
    }

    public Pool[] pools;

    private void Awake()
    {
        Instance = this;

        for (int j = 0; j < pools.Length; j++)
        {
            pools[j].PooledEnemies = new Queue<GameObject>();

            for (int i = 0; i < pools[j].PoolSize; i++)
            {
                GameObject enemy = Instantiate(pools[j].EnemyPrefab);
                enemy.SetActive(false);

                pools[j].PooledEnemies.Enqueue(enemy);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnDelay());
    }

    public GameObject GetEnemies(int enemyType, int spawnIndex)
    {
        if (enemyType >= pools.Length) return null;

        GameObject enemy = pools[enemyType].PooledEnemies.Dequeue();
        enemy.transform.position = transform.GetChild(spawnIndex).transform.position;
        enemy.SetActive(true);
        pools[enemyType].PooledEnemies.Enqueue(enemy);
        return enemy;
    }

    IEnumerator SpawnDelay()
    {
        yield return _spawnDelay;
        int iterationLimit = 6;
        int spawnIndex = 0;

        for (int j = 0; j < iterationLimit; j++)
        {
            int enemyType = UnityEngine.Random.Range(0, 3);
            GetEnemies(enemyType, spawnIndex);
            spawnIndex++;
        }
    }
}
