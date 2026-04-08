using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

  public int kills => GameManager.Instance != null ? GameManager.Instance.GetKills() : 0;

public int level => GameManager.Instance != null ? GameManager.Instance.currentLevel : 1;
    public float timeSurvived;

    void Awake()
    {
        Instance = this;
    }

void Update()
{
    if (Time.timeScale > 0f) // 🔥 stops counting after death
        timeSurvived += Time.deltaTime;
}
    public int GetScore()
    {
        int score =
            (level * 100) +
            (kills * 10) +
            Mathf.FloorToInt(timeSurvived * 2f);

        return score;
    }

    public void SaveHighScore()
{
    int currentScore = GetScore();
    int highScore = PlayerPrefs.GetInt("HIGH_SCORE", 0);

    if (currentScore > highScore)
    {
        PlayerPrefs.SetInt("HIGH_SCORE", currentScore);
        PlayerPrefs.Save();

        Debug.Log("NEW HIGH SCORE SAVED: " + currentScore);
    }
}

public int GetHighScore()
{
    return PlayerPrefs.GetInt("HIGH_SCORE", 0);
}

public void ResetScore()
{
    timeSurvived = 0f;
}
}