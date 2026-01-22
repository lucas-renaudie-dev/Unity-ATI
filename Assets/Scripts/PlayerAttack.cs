using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public UIManager uiManager;
    public PlayerScore playerScore;
    private Animator animator;

    [Header("Hitboxes")]
    public Transform punchHitbox;
    public Transform kickHitbox;

    [Header("Damage")]
    public float punchDamage = 10f;
    public float kickDamage = 20f;

    [Header("Cooldowns")]
    public float punchCooldown = 0.5f;
    public float kickCooldown = 1f;

    [Header("Enemy Layer")]
    public LayerMask enemyLayer;

    private float punchTimer;
    private float kickTimer;
    private bool isAttacking;

    // --- Delay until hit is registered (seconds) ---
    public float punchHitDelay = 0.2f;
    public float kickHitDelay = 0.3f;
    Rigidbody rb;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>(true);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // --- Timers ---
        punchTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;

        // --- UI cooldowns ---
        if (uiManager)
        {
            uiManager.UpdatePunchCooldown(Mathf.Clamp01(punchTimer / punchCooldown));
            uiManager.UpdateKickCooldown(Mathf.Clamp01(kickTimer / kickCooldown));
        }

        // --- Input ---
        if (Input.GetMouseButtonDown(0) && punchTimer <= 0f && !isAttacking && PlayerIsGrounded())
        {
            isAttacking = true;
            punchTimer = punchCooldown;

            ClearAttackTriggers();
            StartCoroutine(PunchRoutine());
        }

        if (Input.GetMouseButtonDown(1) && kickTimer <= 0f && !isAttacking && PlayerIsGrounded())
        {
            isAttacking = true;
            kickTimer = kickCooldown;

            ClearAttackTriggers();
            StartCoroutine(KickRoutine());
        }
    }

    bool PlayerIsGrounded()
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        return movement != null && movement.isGrounded;
    }

    IEnumerator PunchRoutine()
    {
        rb.constraints |= RigidbodyConstraints.FreezeRotationY;
        rb.constraints |= RigidbodyConstraints.FreezePosition;
        GameSceneManager.Instance.inputEnabled = false;

        animator.SetTrigger("crossPunch");

        // wait before hit
        yield return new WaitForSeconds(punchHitDelay);

        // do the actual hit
        Collider[] hits = Physics.OverlapBox(
            punchHitbox.position,
            punchHitbox.localScale * 0.5f,
            punchHitbox.rotation,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (!enemy) continue;

            bool b = enemy.TakeDamage(punchDamage);

            if (playerScore != null && b)
                playerScore.AddKill();
        }

        // end attack after animation finishes
        yield return new WaitForSeconds(0.35f); // optional extra wait to avoid instant reattack
        EndAttack();
        GameSceneManager.Instance.inputEnabled = true;
        rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
        rb.constraints &= ~RigidbodyConstraints.FreezePosition;
    }

    IEnumerator KickRoutine()
    {
        rb.constraints |= RigidbodyConstraints.FreezeRotationY;
        rb.constraints |= RigidbodyConstraints.FreezePosition;
        GameSceneManager.Instance.inputEnabled = false;

        animator.SetTrigger("kick");

        // wait before hit
        yield return new WaitForSeconds(kickHitDelay);

        Collider[] hits = Physics.OverlapBox(
            kickHitbox.position,
            kickHitbox.localScale * 0.5f,
            kickHitbox.rotation,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (!enemy) continue;

            bool b = enemy.TakeDamage(kickDamage);

            if (playerScore != null && b)
                playerScore.AddKill();
        }

        yield return new WaitForSeconds(0.35f);
        EndAttack();
        GameSceneManager.Instance.inputEnabled = true;
        rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
        rb.constraints &= ~RigidbodyConstraints.FreezePosition;
    }

    void Punch() 
    {
        animator.SetTrigger("crossPunch");

        Collider[] hits = Physics.OverlapBox(
            punchHitbox.position,
            punchHitbox.localScale * 0.5f,
            punchHitbox.rotation,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (!enemy) continue;

            bool b = enemy.TakeDamage(punchDamage);

            // Add score/combo
            if (playerScore != null && b)
            {
                playerScore.AddKill();
            }
                
        }
    }

    void Kick()
    {
        animator.SetTrigger("kick");

        Collider[] hits = Physics.OverlapBox(
            kickHitbox.position,
            kickHitbox.localScale * 0.5f,
            kickHitbox.rotation,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (!enemy) continue;

            bool b = enemy.TakeDamage(kickDamage);

            // Add score/combo
            if (playerScore != null && b)
            {
                playerScore.AddKill();
            }
                
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        ClearAttackTriggers();
    }
    void ClearAttackTriggers()
    {
        animator.ResetTrigger("crossPunch");
        animator.ResetTrigger("kick");
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (punchHitbox)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = punchHitbox.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        if (kickHitbox)
        {
            Gizmos.color = Color.blue;
            Gizmos.matrix = kickHitbox.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
#endif
}
