using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public TMP_Text killsValueText;
    public TMP_Text gemsValueText;
    public TMP_Text runsValueText;
    public TMP_Text timeValueText;
    public TMP_Text highScoreValueText;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        killsValueText.text = PlayerStatsManager.GetKills().ToString();
        gemsValueText.text = PlayerStatsManager.GetGems().ToString();
        runsValueText.text = PlayerStatsManager.GetRuns().ToString();

        float time = PlayerStatsManager.GetTime();

        int h = Mathf.FloorToInt(time / 3600);
        int m = Mathf.FloorToInt((time % 3600) / 60);
        int s = Mathf.FloorToInt(time % 60);

        timeValueText.text = $"{h:00}:{m:00}:{s:00}";

        // 🔥 HIGH SCORE
        highScoreValueText.text = PlayerStatsManager.GetHighScore().ToString();
    }
}