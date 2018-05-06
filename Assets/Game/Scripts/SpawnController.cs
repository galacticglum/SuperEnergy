using System.Linq;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameObject[] spawnObjects;
    [SerializeField]
    private float spawnRadius = 20;

    private GameObject spawnParent;

    private int currentWaveSize = 1;

    [SerializeField]
    private float timeUntilNextSpawn = 1.0f;

    private void Start()
    {
        spawnParent = new GameObject("Spawn_Parent");
    }

    private void Update()
    {
        timeUntilNextSpawn -= Time.deltaTime;
        if (timeUntilNextSpawn > 0.0f)
        {
            return;
        }

        int objectIndex = GetRandomSpawnObject();

        Vector3 offset = Random.onUnitSphere;
        offset.z = 0;
        offset = offset.normalized * spawnRadius;

        GameObject enemyGameObject = Enemy.Create(spawnObjects[objectIndex], transform.position + offset, playerController);
        enemyGameObject.transform.SetParent(spawnParent.transform);

        --currentWaveSize;

        timeUntilNextSpawn = Random.Range(GetNewMinTime(), GetNewMaxTime());
        currentWaveSize = Random.Range(GetNewMinWave(), GetNewMaxWave());
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
}
