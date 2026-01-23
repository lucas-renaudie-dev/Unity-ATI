using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Score")]
    public TMP_Text scoreText;
    public TMP_Text comboText;

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

    // --- Score ---
    public void UpdateScore(int score)
    {
        if (scoreText)
            scoreText.text = "Score: " + score;
    }

    // --- Combo ---
    public void UpdateCombo(int combo)
    {
        if (!comboText) return;

        if (combo > 1)
        {
            comboText.text = "COMBO x" + combo;

            if (comboCoroutine != null)
                StopCoroutine(comboCoroutine);

            comboCoroutine = StartCoroutine(ShowComboPopup());
        }
        else
        {
            comboText.text = "";
        }
    }

    // --- Combo pop-up animation ---
    private IEnumerator ShowComboPopup()
    {
        float zoomTime = 0.2f;
        float displayTime = 0.6f;
        float fadeTime = 0.2f;

        // Init
        comboText.transform.localScale = Vector3.one * 0.5f;
        comboText.alpha = 1f;

        // Scale up
        float timer = 0f;
        while (timer < zoomTime)
        {
            timer += Time.deltaTime;
            float t = timer / zoomTime;
            comboText.transform.localScale =
                Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.5f, t);
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        // Fade out + shrink
        timer = 0f;
        Vector3 startScale = comboText.transform.localScale;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            comboText.transform.localScale =
                Vector3.Lerp(startScale, Vector3.one * 0.5f, t);
            comboText.alpha = 1f - t;
            yield return null;
        }

        comboText.alpha = 0f;
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
    public void UpdatePunchCooldown(float ratio)
    {
        if (punchCooldownBar)
            punchCooldownBar.fillAmount = 1f - ratio;
    }

    public void UpdateKickCooldown(float ratio)
    {
        if (kickCooldownBar)
            kickCooldownBar.fillAmount = 1f - ratio;
    }
}
