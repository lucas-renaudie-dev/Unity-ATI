using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.Experimental.GlobalIllumination;
public class EnemyController : MonoBehaviour
{
    public float minMoveSpeed = 2f;
    public float maxMoveSpeed = 5f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public int health = 20;

    private Transform player;
    private Rigidbody rb;
    private Animator animator;
    public bool isDead { get; private set; } = false;
    public bool isGettingKnockedBack { get; private set; } = false;
    public bool isAttacking = false;
    public float knockDelay = 0.5f;
    public GameObject light;
    private float moveSpeed;
    
    void Awake()
    {
        animator = GetComponentInChildren<Animator>(true);
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        var settings = GameDifficultySettings.Instance;

        minMoveSpeed = settings.minMoveSpeed;
        maxMoveSpeed = settings.maxMoveSpeed;
        health = settings.enemyHealth;

        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    void Update()
    {
        if (!player || isDead || isGettingKnockedBack || isAttacking) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        float distance = dir.magnitude;

        if (distance <= attackRange)
        {
            isAttacking = true;
            animator.SetTrigger("punch");
        }
    }

    void FixedUpdate()
    {
        if (!player) return;
        if (isDead) {
            Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>());
            gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
            light.gameObject.SetActive(false);
            return;
        }
        if (isGettingKnockedBack) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        float distance = dir.magnitude;

        // --- MOVEMENT ---
        if (distance > attackRange && !isAttacking)
        {
            isAttacking = false;
            animator.SetBool("isMoving", true);
            dir.Normalize();

            rb.linearVelocity = new Vector3(
                dir.x * moveSpeed,
                rb.linearVelocity.y,
                dir.z * moveSpeed
            );
        }
        else
        {
            animator.SetBool("isMoving", false);
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        // --- ROTATION ---
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
            rb.rotation = Quaternion.RotateTowards(
                rb.rotation,
                targetRotation,
                720f * Time.fixedDeltaTime
            );
        }
    }

    public bool TakeDamage(float dmg)
    {
        if (isDead) return false;

        health -= Mathf.RoundToInt(dmg);

        if (health <= 0)
        {
            isDead = true;

            // Trigger fall/death animation
            animator.SetTrigger("fall");

            // Add kill score
            PlayerScore playerScore = GameObject.FindWithTag("Player")
                                               ?.GetComponent<PlayerScore>();
            if (playerScore != null)
            {
                playerScore.AddKill();
            }

            // Destroy after 3 seconds
            StartCoroutine(DestroyAfterDelay(3f));
            return true;
        }
        else
        {
            // Trigger knockback animation
            isGettingKnockedBack = true;
            animator.SetTrigger("knockBack");
            animator.SetBool("isGettingKnocked", true); 
            isAttacking = false;
            StartCoroutine(EnableRotationAfter(knockDelay));
        }

        return false;
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private IEnumerator EnableRotationAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        isGettingKnockedBack = false;
        animator.SetBool("isGettingKnocked", false);    
    }
}