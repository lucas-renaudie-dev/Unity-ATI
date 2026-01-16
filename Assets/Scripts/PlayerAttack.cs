using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
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
        punchTimer -= Time.deltaTime;
        kickTimer -= Time.deltaTime;

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
            Debug.Log("Punch hit " + hit.name + " for " + punchDamage);
            hit.GetComponent<EnemyController>().TakeDamage(punchDamage);
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
            Debug.Log("Kick hit " + hit.name + " for " + kickDamage);
            hit.GetComponent<EnemyController>().TakeDamage(kickDamage);
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
