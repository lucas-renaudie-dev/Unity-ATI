using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health;
    public float regenDelay = 3f;
    public float regenRate = 5f;

    private float lastHitTime;
    public UIManager uiManager;

    void Awake()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (Time.time - lastHitTime > regenDelay && health < maxHealth)
        {
            health += regenRate * Time.deltaTime;
            health = Mathf.Min(health, maxHealth);
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        lastHitTime = Time.time;

        // Optional: flash red screen
        Debug.Log("Player hit: " + dmg + " remaining health: " + health);

        if (health <= 0)
        {
            Debug.Log("Player Died");
            // TODO: Game over logic
        }

        if(uiManager != null)
            uiManager.UpdateHealth(health, maxHealth);

        if(uiManager != null)
            uiManager.ShowDamage(0.7f); // intensity of red flash
    }
}
