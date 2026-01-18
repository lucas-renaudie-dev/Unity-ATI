using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public UIManager uiManager;
    public PlayerScore playerScore; // assign in Inspector

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
        if (Input.GetMouseButtonDown(0) && punchTimer <= 0f)
        {
            Punch();
            punchTimer = punchCooldown;
        }

        if (Input.GetMouseButtonDown(1) && kickTimer <= 0f)
        {
            Kick();
            kickTimer = kickCooldown;
        }
    }

    void Punch()
    {
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
            //Debug.Log("Punch hit " + hit.name + " for " + punchDamage);

            // Add score/combo
            if (playerScore != null && b)
            {
                playerScore.AddKill();
                Debug.Log("Score after punch: " + playerScore.GetScore());
            }
                
        }
    }

    void Kick()
    {
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
            //Debug.Log("Kick hit " + hit.name + " for " + kickDamage);

            // Add score/combo
            if (playerScore != null && b)
            {
                playerScore.AddKill();
                Debug.Log("Score after kick: " + playerScore.GetScore());
            }
                
        }
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
