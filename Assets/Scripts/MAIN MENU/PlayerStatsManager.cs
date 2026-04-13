using UnityEngine;

public static class PlayerStatsManager
{
    const string KILLS = "TOTAL_KILLS";
    const string GEMS = "TOTAL_GEMS";
    const string RUNS = "TOTAL_RUNS";
    const string TIME = "TOTAL_PLAYTIME";
    const string HIGH_SCORE = "STATS_HIGH_SCORE";

    // ================= ADD =================

    public static void AddKill(int amount = 1)
    {
        PlayerPrefs.SetInt(KILLS, GetKills() + amount);
    }

    public static void AddGems(int amount)
    {
        PlayerPrefs.SetInt(GEMS, GetGems() + amount);
    }

    public static void AddRun()
    {
        PlayerPrefs.SetInt(RUNS, GetRuns() + 1);
    }

    public static void AddPlayTime(float seconds)
    {
        PlayerPrefs.SetFloat(TIME, GetTime() + seconds);
    }

    // 🔥 SAVE HIGH SCORE FROM GAME
    public static void TrySetHighScore(int score)
    {
        if (score > GetHighScore())
        {
            PlayerPrefs.SetInt(HIGH_SCORE, score);
            Debug.Log("🏆 NEW GLOBAL HIGH SCORE: " + score);
        }
    }

    // ================= GET =================

    public static int GetKills() => PlayerPrefs.GetInt(KILLS, 0);
    public static int GetGems() => PlayerPrefs.GetInt(GEMS, 0);
    public static int GetRuns() => PlayerPrefs.GetInt(RUNS, 0);
    public static float GetTime() => PlayerPrefs.GetFloat(TIME, 0f);
    public static int GetHighScore() => PlayerPrefs.GetInt(HIGH_SCORE, 0);
}