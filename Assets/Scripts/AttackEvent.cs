using UnityEngine;

public class AttackEvent : MonoBehaviour
{
    private Transform player;
    private EnemyController enemyController;
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform; // Make sure Player has tag "Player"
        enemyController = GetComponentInParent<EnemyController>();
    }

    public void Attack()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        float distance = dir.magnitude;

        if (distance <= enemyController.attackRange)
            playerHealth.TakeDamage(enemyController.attackDamage);
    }

    public void EndAttack()
    {
        enemyController.isAttacking = false;
    }
}
