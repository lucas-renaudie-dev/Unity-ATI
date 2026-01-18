using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    // Combo pop-up coroutine
    private Coroutine comboCoroutine;

    void Update()
    {
        // --- Fade damage overlay ---
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
        {
            if (combo > 1)
            {
                comboText.text = "Combo x" + combo;

                // Stop previous animation if running
                if (comboCoroutine != null)
                    StopCoroutine(comboCoroutine);

                // Start pop-up animation
                comboCoroutine = StartCoroutine(ShowComboPopup());
            }
            else
            {
                comboText.text = "";
            }
        }
    }

    // --- Combo pop-up animation ---
    private IEnumerator ShowComboPopup()
    {
        float zoomTime = 0.2f;      // time to scale up
        float displayTime = 0.6f;   // stay on screen
        float fadeTime = 0.2f;      // fade out duration

        // --- Initialize ---
        comboText.transform.localScale = Vector3.one * 0.5f;
        comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 1f);

        // --- Scale up ---
        float timer = 0f;
        while (timer < zoomTime)
        {
            timer += Time.deltaTime;
            float t = timer / zoomTime;
            comboText.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.5f, t);
            yield return null;
        }
        comboText.transform.localScale = Vector3.one * 1.5f;

        // --- Stay visible ---
        yield return new WaitForSeconds(displayTime);

        // --- Fade out and shrink ---
        timer = 0f;
        Vector3 startScale = comboText.transform.localScale;
        Color startColor = comboText.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            comboText.transform.localScale = Vector3.Lerp(startScale, Vector3.one * 0.5f, t);
            comboText.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            yield return null;
        }

        comboText.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        comboText.transform.localScale = Vector3.one * 0.5f;
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
