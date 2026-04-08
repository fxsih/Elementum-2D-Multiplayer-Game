using UnityEngine;
using TMPro;
using System.Collections;

public class DeathPanelUI : MonoBehaviour
{
    [Header("Main Score")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    [Header("Stats")]
    public TMP_Text killsText;
    public TMP_Text timeText;
    public TMP_Text levelText;

    void OnEnable()
    {
        StartCoroutine(ShowSequence());
    }

    IEnumerator ShowSequence()
    {
        if (ScoreManager.Instance == null || GameManager.Instance == null)
            yield break;

        int score = ScoreManager.Instance.GetScore();
        int highScore = ScoreManager.Instance.GetHighScore();
        int oldHighScore = PlayerPrefs.GetInt("LAST_HIGH_SCORE", 0);

        int kills = GameManager.Instance.GetKills();
        int level = GameManager.Instance.currentLevel;
        float time = ScoreManager.Instance.timeSurvived;

        // 🔥 RESET UI
        scoreText.text = "0";
        highScoreText.text = highScore.ToString();
        killsText.text = "0";
        levelText.text = "00";
        timeText.text = "00:00";

        yield return new WaitForSecondsRealtime(0.2f);

        // 🔥 1. KILLS
        yield return StartCoroutine(AnimateNumber(killsText, kills));

        // 🔥 2. TIME
        yield return StartCoroutine(AnimateTime(timeText, time));

        // 🔥 3. LEVEL
        yield return StartCoroutine(AnimateLevel(level));

        // 🔥 4. SCORE
        yield return StartCoroutine(AnimateNumber(scoreText, score, 0.8f));

        // 🔥 HIGH SCORE SYNC FIX
        if (score > oldHighScore)
        {
            scoreText.color = new Color(1f, 0.84f, 0f);

            // 🔥 animate high score ALSO
            yield return StartCoroutine(AnimateNumber(highScoreText, score, 0.6f));
        }
        else
        {
            scoreText.color = Color.cyan;
            highScoreText.text = highScore.ToString();
        }
    }

    IEnumerator AnimateNumber(TMP_Text text, int finalValue, float duration = 0.5f)
    {
        int current = 0;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Pow(timer / duration, 0.7f);
            int value = Mathf.FloorToInt(Mathf.Lerp(0, finalValue, t));

            text.text = value.ToString();

            yield return null;
        }

        text.text = finalValue.ToString();
    }

    IEnumerator AnimateLevel(int finalLevel)
    {
        float duration = 0.5f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Pow(timer / duration, 0.7f);
            int value = Mathf.FloorToInt(Mathf.Lerp(0, finalLevel, t));

            levelText.text = value.ToString("00");

            yield return null;
        }

        levelText.text = finalLevel.ToString("00");
    }

    IEnumerator AnimateTime(TMP_Text text, float finalTime, float duration = 0.6f)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Pow(timer / duration, 0.7f);
            float currentTime = Mathf.Lerp(0, finalTime, t);

            int min = Mathf.FloorToInt(currentTime / 60f);
            int sec = Mathf.FloorToInt(currentTime % 60f);

            text.text = $"{min:00}:{sec:00}";

            yield return null;
        }

        int finalMin = Mathf.FloorToInt(finalTime / 60f);
        int finalSec = Mathf.FloorToInt(finalTime % 60f);

        text.text = $"{finalMin:00}:{finalSec:00}";
    }
}