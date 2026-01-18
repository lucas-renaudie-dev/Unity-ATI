using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public int health = 20;

    private Transform player;
    private float attackTimer = 0f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform; // Make sure Player has tag "Player"
    }

    void FixedUpdate()
    {
        if (!player) return;

        Vector3 dir = (player.position - transform.position);
        dir.y = 0;

        // Move towards player if not in attack range
        if (dir.magnitude > attackRange)
        {
            dir.Normalize();
            rb.linearVelocity = dir * moveSpeed + Vector3.up * rb.linearVelocity.y;
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            // Attack only if cooldown finished
            attackTimer -= Time.fixedDeltaTime;
            if (attackTimer <= 0f)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    attackTimer = attackCooldown; // reset cooldown
                }
            }
        }

        // Rotate towards player
        if (dir.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, 720f * Time.fixedDeltaTime);
        }
    }


    public bool TakeDamage(float dmg)
    {
        health -= (int)dmg;
        if (health <= 0)
        {
            PlayerScore playerScore = GameObject.FindWithTag("Player").GetComponent<PlayerScore>();
            if (playerScore != null)
            {
                playerScore.AddKill();
            }
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }
}
