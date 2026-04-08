using UnityEngine;

public class AltarHeal : MonoBehaviour
{
    public float healPerSecond = 15f;
    public ParticleSystem particles;

    PlayerController player;
    bool isHealing = false;

    bool lastActiveState = false;

   void Update()
{
    bool active = GameManager.Instance.altarUses > 0;

    if (particles != null && active != lastActiveState)
    {
        if (active)
        {
            particles.gameObject.SetActive(true);
            particles.Clear();
            particles.Play();
        }
        else
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.Clear();
            particles.gameObject.SetActive(false);
        }

        lastActiveState = active;
    }
}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isHealing) return;
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.altarUses <= 0) return;

        player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.GetCurrentHealth() >= player.maxHealth)
            return;

        isHealing = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isHealing || player == null) return;

        player.Heal(healPerSecond * Time.deltaTime);

        if (player.GetCurrentHealth() >= player.maxHealth)
        {
            isHealing = false;
            ConsumeUse();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!isHealing || player == null) return;

        ConsumeUse();
    }

    void ConsumeUse()
    {
        if (GameManager.Instance.altarUses <= 0) return;

        GameManager.Instance.altarUses = 0;
        isHealing = false;
        player = null;
    }
}