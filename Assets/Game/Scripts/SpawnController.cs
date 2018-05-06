using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct SpawnPoint
{
    public Transform Transform;
    public float Weight;
}

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameObject[] spawnObjects;
    [SerializeField]
    private SpawnPoint[] spawnPoints;
    [SerializeField]
    private float timeBetweenWave = 3;

    private GameObject spawnParent;
    private int currentWaveSize;
    private bool isCurrentlyWave;

    private float timeUntilNextWave;
    private float timeUntilNextSpawn = 1;

    private void Start()
    {
        spawnParent = new GameObject("Spawn_Parent");
        timeUntilNextWave = timeBetweenWave;
    }

    private void Update()
    {
        int enemyAliveCount = FindObjectsOfType<Enemy>().Length;
        if (enemyAliveCount == 0 && currentWaveSize == 0)
        {
            if (isCurrentlyWave)
            {
                Debug.Log("Wave ended");
                timeUntilNextWave = timeBetweenWave;
                isCurrentlyWave = false;
            }

            timeUntilNextWave -= Time.deltaTime;
            if (timeUntilNextWave > 0) return;

            currentWaveSize = Random.Range(GetNewMinWave(), GetNewMaxWave());
            isCurrentlyWave = true;
            Debug.Log("Wave started");
        }

        if (!isCurrentlyWave || currentWaveSize == 0) return;

        timeUntilNextSpawn -= Time.deltaTime;
        if (timeUntilNextSpawn > 0)
        {
            return;
        }

        int objectIndex = GetRandomSpawnObject();
        int locationIndex = GetRandomSpawnLocation();

        GameObject enemyGameObject = Enemy.Create(spawnObjects[objectIndex], spawnPoints[locationIndex].Transform.position, playerController);
        enemyGameObject.transform.SetParent(spawnParent.transform);

        --currentWaveSize;

        timeUntilNextSpawn = Random.Range(GetNewMinTime(), GetNewMaxTime());
    }

    private static int GetRandomWeightedIndex(float[] weights)
    {
        float totalweight = weights.Sum();
        float randomValue = Random.Range(0.0f, totalweight);

        int index = 0;
        float accumulator = 0.0f;
        while (index < weights.Length)
        {
            accumulator += weights[index];
            if (accumulator >= randomValue)
            {
                return index;
            }

            ++index;
        }

        return weights.Length - 1;
    }

    private int GetRandomSpawnObject()
    {
        float[] objectWeights;
        if (Time.time < 20.0f)
        {
            objectWeights = new[] { 1.0f };
        }
        else if (Time.time < 60.0f)
        {
            objectWeights = new[] { 1.0f };
        }
        else
        {
            objectWeights = new[] { 1.0f };
        }

        return Mathf.Clamp(GetRandomWeightedIndex(objectWeights), 0, spawnObjects.Length - 1);
    }

    private float GetNewMinTime()
    {
        if (currentWaveSize > 0)
        {
            return 1.0f;
        }

        if (Time.time < 5.0f)
        {
            return 3.0f;
        }

        return Time.time < 20.0f ? 6.0f : 8.0f;
    }

    private float GetNewMaxTime()
    {
        if (currentWaveSize > 0)
        {
            return 3.0f;
        }

        if (Time.time < 5.0f)
        {
            return 6.0f;
        }

        return Time.time < 20.0f ? 10.0f : 12.0f;
    }

    private int GetNewMinWave()
    {
        if (currentWaveSize > 0)
        {
            return currentWaveSize;
        }

        if (Time.time < 5.0f)
        {
            return 1;
        }

        return Time.time < 20.0f ? 2 : 3;
    }

    private int GetNewMaxWave()
    {
        if (currentWaveSize > 0)
        {
            return currentWaveSize;
        }

        if (Time.time < 5.0f)
        {
            return 5;
        }

        if (Time.time < 20.0f)
        {
            return 6;
        }

        return Time.time < 30.0f ? 8 : 12;
    }

    private int GetRandomSpawnLocation()
    {
        float[] locationWeights = new float[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            locationWeights[i] = spawnPoints[i].Weight;
        }

        return Mathf.Clamp(GetRandomWeightedIndex(locationWeights), 0, spawnPoints.Length - 1);
    }
}
