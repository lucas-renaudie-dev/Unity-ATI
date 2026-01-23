using UnityEngine;
using UnityEngine.UI;

public class DifficultyScrollbarUI : MonoBehaviour
{
    public Scrollbar difficultyScrollbar;

    void Start()
    {
        // Convert current difficulty to scrollbar value
        difficultyScrollbar.value = DifficultyToScrollbarValue(
            GameDifficultySettings.Instance.difficulty
        );
    }

    void Update()
    {
        GameDifficultySettings.Instance.difficulty = ScrollbarValueToDifficulty(difficultyScrollbar.value);
    }

    // Hook this to Scrollbar → On Value Changed (float)
    public void OnDifficultyChanged(float value)
    {
        
    }

    Difficulty ScrollbarValueToDifficulty(float value)
    {
        if (value < 0.25f)
            return Difficulty.Easy;
        else if (value < 0.75f)
            return Difficulty.Medium;
        else
            return Difficulty.Hard;
    }

    float DifficultyToScrollbarValue(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                return 0f;
            case Difficulty.Medium:
                return 0.5f;
            case Difficulty.Hard:
                return 1f;
            default:
                return 0.5f;
        }
    }
}
