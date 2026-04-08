using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    public float duration = 4f;
    public float damagePerSecond = 5f;

    [Header("Leniency")]
    public float delayBeforeDamage = 0.5f; // 🔥 grace time

    float timer;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 🔥 Fade out
        float t = timer / duration;
        float alpha = Mathf.Lerp(1f, 0f, t);

        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (timer < delayBeforeDamage) return; // 🔥 leniency check

        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ApplyPoison(damagePerSecond, 5f);
        }
    }
}