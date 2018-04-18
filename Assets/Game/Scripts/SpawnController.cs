using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private float spawnCooldown = 20;
    [SerializeField]
    private float spawnChance = 0.6f;
    private float lastSpawnTime;
    private int spawnerIndex;

    private void Update()
    {
        if (!(Time.time > lastSpawnTime + spawnCooldown) || !(Random.value < spawnChance)) return;

        lastSpawnTime = Time.time;
        Enemy.Create(enemyPrefab, spawnPoints[spawnerIndex++ % spawnPoints.Length].position, playerController);
    }
}
