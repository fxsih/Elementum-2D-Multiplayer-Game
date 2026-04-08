using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    void Update()
    {
        float time = GameManager.Instance.gameTime;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}