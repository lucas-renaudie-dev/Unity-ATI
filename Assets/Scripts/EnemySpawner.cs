using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform player;
    public Transform arenaRoot; // parent object that contains the 8 wall objects

    [Header("Spawn Settings")]
    public float minSpawnDistance = 10f;
    public float maxSpawnDistance = 30f;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;
    public int maxSpawnAttempts = 20;

    private float timer;
    private Bounds arenaBounds;

    void Start()
    {
        timer = spawnInterval;
        CalculateArenaBounds();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    void CalculateArenaBounds()
    {
        BoxCollider[] wallColliders = arenaRoot.GetComponentsInChildren<BoxCollider>();

        if (wallColliders.Length < 1)
        {
            Debug.LogError("No BoxColliders found under arenaRoot!");
            return;
        }

        arenaBounds = wallColliders[0].bounds;

        for (int i = 1; i < wallColliders.Length; i++)
        {
            arenaBounds.Encapsulate(wallColliders[i].bounds);
        }
    }

    void SpawnEnemy()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemies)
            return;

        Vector3 spawnPos = Vector3.zero;
        bool found = false;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 candidate = player.position + new Vector3(
                direction.x * distance,
                0f,
                direction.y * distance
            );

            if (arenaBounds.Contains(candidate))
            {
                spawnPos = candidate;
                found = true;
                break;
            }
        }

        if (!found)
            return;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.tag = "Enemy";
    }

    void OnDrawGizmosSelected()
    {
        if (player == null || arenaRoot == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, maxSpawnDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(arenaBounds.center, arenaBounds.size);
    }
}
