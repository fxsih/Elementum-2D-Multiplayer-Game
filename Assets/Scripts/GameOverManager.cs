using UnityEngine;
using EasyTransition;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    public GameObject deathPanel;
    public TransitionSettings transition;
    public float deathPanelDelay = 0.8f;

    public GameObject highScorePanel;
public TMP_Text highScoreValueText;
bool waitingForInput = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
{
    if (waitingForInput && Input.anyKeyDown)
    {
        waitingForInput = false;

        if (highScorePanel != null)
            highScorePanel.SetActive(false);

        StartCoroutine(ShowDeathPanelDelayed());
    }
}
  public void GameOver()
{
    Time.timeScale = 0.2f;

    int score = ScoreManager.Instance.GetScore();
    int oldHighScore = ScoreManager.Instance.GetHighScore();

PlayerPrefs.SetInt("LAST_HIGH_SCORE", oldHighScore);

// 🔥 SAVE FIRST
ScoreManager.Instance.SaveHighScore();

// 🔥 NOW updated value exists
int newHighScore = ScoreManager.Instance.GetHighScore();

    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    bool isNewHighScore = score > oldHighScore;

    if (isNewHighScore)
    {
        ShowHighScoreScreen(score);
    }
    else
    {
        StartCoroutine(ShowDeathPanelDelayed());
    }
}
void ShowHighScoreScreen(int score)
{
    if (highScorePanel != null)
    {
        highScorePanel.SetActive(true);

        if (highScoreValueText != null)
            highScoreValueText.text = score.ToString();
    }

    waitingForInput = true;
}

   IEnumerator ShowDeathPanelDelayed()
{
    yield return new WaitForSecondsRealtime(0.2f);
    deathPanel.SetActive(true);
}
   public void Respawn()
{
    StartCoroutine(RespawnRoutine());
}

IEnumerator RespawnRoutine()
{
    Time.timeScale = 1f;

    yield return null;

    var tm = EasyTransition.TransitionManager.Instance();

    if (tm != null && transition != null)
    {
        tm.Transition(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            transition,
            0f
        );
    }
    else
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(MenuRoutine());
    }

    IEnumerator MenuRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        var tm = TransitionManager.Instance();

        if (tm != null && transition != null)
        {
            tm.Transition("MainMenu", transition, 0f);
        }
        else
        {
            Debug.LogError("❌ Transition failed → fallback load");
            SceneManager.LoadScene("MainMenu");
        }
    }
}