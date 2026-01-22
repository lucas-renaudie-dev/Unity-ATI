using UnityEngine;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class GameDifficultySettings : MonoBehaviour
{
    public static GameDifficultySettings Instance;

    [Header("Difficulty")]
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Enemy Spawner Settings")]
    public float spawnInterval;
    public int maxEnemies;
    public float minSpawnDistance;
    public float maxSpawnDistance;

    [Header("Enemy Stats")]
    public float minMoveSpeed;
    public float maxMoveSpeed;
    public int enemyHealth;
    public float enemyDamage;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ApplyDifficulty();
    }

    public void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                spawnInterval = 2f;
                maxEnemies = 5;

                minMoveSpeed = 1f;
                maxMoveSpeed = 5f;
                enemyHealth = 10;
                break;

            case Difficulty.Medium:
                spawnInterval = 1f;
                maxEnemies = 15;

                minMoveSpeed = 3f;
                maxMoveSpeed = 7f;
                enemyHealth = 20;
                break;

            case Difficulty.Hard:
                spawnInterval = 0.5f;
                maxEnemies = 30;

                minMoveSpeed = 5f;
                maxMoveSpeed = 9f;
                enemyHealth = 20;
                break;
        }
    }
}
