using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Score")]
    public Text scoreText;
    public Text comboText;

    [Header("Health")]
    public Image healthBar;
    public Image damageOverlay;
    public float overlayFadeSpeed = 2f;

    [Header("Cooldowns")]
    public Image punchCooldownBar;
    public Image kickCooldownBar;

    private float overlayAlpha = 0f;

    void Update()
    {
        // Fade damage overlay
        if (overlayAlpha > 0f)
        {
            overlayAlpha -= overlayFadeSpeed * Time.deltaTime;
            damageOverlay.color = new Color(1f, 0f, 0f, overlayAlpha);
        }
    }

    // --- Score/Combo ---
    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = "Score: " + score;
    }

    public void UpdateCombo(int combo)
    {
        if (comboText)
            comboText.text = combo > 1 ? "Combo x" + combo : "";
    }

    // --- Health ---
    public void UpdateHealth(float current, float max)
    {
        if (healthBar)
            healthBar.fillAmount = current / max;
    }

    public void ShowDamage(float intensity = 0.5f)
    {
        overlayAlpha = Mathf.Clamp01(intensity);
        if (damageOverlay)
            damageOverlay.color = new Color(1f, 0f, 0f, overlayAlpha);
    }

    // --- Cooldowns ---
    public void UpdatePunchCooldown(float ratio) // 0 = ready, 1 = full cooldown
    {
        if (punchCooldownBar) punchCooldownBar.fillAmount = 1f - ratio;
    }

    public void UpdateKickCooldown(float ratio)
    {
        if (kickCooldownBar) kickCooldownBar.fillAmount = 1f - ratio;
    }
}
