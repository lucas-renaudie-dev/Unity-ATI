using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{   
    public int pointsPerKill = 100;         // Base points per enemy
    public float comboTime = 3f;            // Time window for combo in seconds
    public TMP_Text scoreText;           // UI text to show score
    public TMP_Text comboText;           // Optional UI to show current combo

    private int totalScore = 0;
    public TMP_Text highScoreText;
    public int highScore { get; private set; } = 0;
    private int comboCount = 0;
    private float comboTimer = 0f;
    public UIManager uiManager;
    
    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Update()
    {
        // Countdown combo timer
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            comboCount = 0; // combo expired
        }

        // Update UI
        if (scoreText)
            scoreText.text = "SCORE\n" + totalScore;
        if (comboText)
            comboText.text = comboCount > 1 ? "COMBO x" + comboCount : "";
        if (highScoreText)
            highScoreText.text = "HIGH SCORE\n" + highScore;
    }

    public void AddKill()
    {
        // Increase combo
        if (comboTimer > 0)
            comboCount++;
        else
            comboCount = 1;

        comboTimer = comboTime;

        // Calculate points
        int pointsEarned = pointsPerKill * comboCount;
        totalScore += pointsEarned;
        setHighScore(totalScore);

        if(uiManager != null)
        {
            uiManager.UpdateScore(totalScore);
            uiManager.UpdateCombo(comboCount);
        }
    }

    public int GetScore()
    {
        return totalScore;
    }

    public void setHighScore(int totalScore)
    {
        if (totalScore > highScore)
        {
            highScore = totalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
}
