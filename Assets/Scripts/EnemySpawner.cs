using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform arenaRoot; // parent object that contains the wall colliders

    private Transform player;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 10f;
    public float maxSpawnDistance = 30f;
    public int maxSpawnAttempts = 20;

    private float spawnInterval;
    private int maxEnemies;

    private float timer;
    private Bounds arenaBounds;
    private bool initialized = false;

    void OnEnable()
    {
        // Listen for difficulty being applied
        GameDifficultySettings.OnDifficultyApplied += InitSpawner;
    }

    void OnDisable()
    {
        GameDifficultySettings.OnDifficultyApplied -= InitSpawner;
    }

    void Start()
    {
        Debug.Log("✅ Arena scene STARTED");

        FindPlayer();
        CalculateArenaBounds();

        // Safe attempt in case difficulty was already applied
        InitSpawner();
    }

    void Update()
    {
        if (!initialized)
            return;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    // ---------------- INIT ----------------

    void InitSpawner()
    {
        if (GameDifficultySettings.Instance == null)
            return;

        spawnInterval = Mathf.Max(0.1f, GameDifficultySettings.Instance.spawnInterval);
        maxEnemies = GameDifficultySettings.Instance.maxEnemies;

        timer = spawnInterval;
        initialized = true;

        Debug.Log($"🟢 Spawner initialized | interval={spawnInterval}, maxEnemies={maxEnemies}");
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void CalculateArenaBounds()
    {
        if (arenaRoot == null)
        {
            Debug.LogError("❌ ArenaRoot not assigned!");
            return;
        }

        BoxCollider[] wallColliders = arenaRoot.GetComponentsInChildren<BoxCollider>();

        if (wallColliders.Length == 0)
        {
            Debug.LogError("❌ No wall colliders found under ArenaRoot!");
            return;
        }

        arenaBounds = wallColliders[0].bounds;

        for (int i = 1; i < wallColliders.Length; i++)
            arenaBounds.Encapsulate(wallColliders[i].bounds);
    }

    // ---------------- SPAWNING ----------------

    void SpawnEnemy()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemies)
            return;

        Vector3 spawnPos = Vector3.zero;
        bool found = false;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 candidate = player.position + new Vector3(
                dir.x * distance,
                0f,
                dir.y * distance
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

    // ---------------- GIZMOS ----------------

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
