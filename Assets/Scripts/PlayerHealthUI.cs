using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image frontFill; // real health
    public Image backFill;  // damage bar

    [Header("Delay Settings")]
    public float delaySpeed = 3f;
    public float delayWait = 0.25f;

    PlayerController player;

    float backFillAmount;
    float delayTimer;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        if (player != null)
        {
            float full = 1f;
            frontFill.fillAmount = full;
            backFill.fillAmount = full;
            backFillAmount = full;
        }
    }

    void Update()
    {
        if (player == null) return;

        float current = player.GetCurrentHealth();
        float max = player.GetMaxHealth();

        float target = current / max;

        // 🔥 FRONT = ALWAYS EXACT HEALTH
        frontFill.fillAmount = target;

        // 🔥 IF DAMAGE (back is higher than front)
        if (backFillAmount > target)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= delayWait)
            {
                backFillAmount = Mathf.MoveTowards(
                    backFillAmount,
                    target,
                    delaySpeed * Time.deltaTime
                );
            }
        }
        else
        {
            // 🔥 HEAL → SNAP instantly
            backFillAmount = target;
            delayTimer = 0f;
        }

        backFill.fillAmount = backFillAmount;
    }
}