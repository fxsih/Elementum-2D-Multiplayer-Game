using UnityEngine;
using System.Collections;

public class RuneGlowController : MonoBehaviour
{
    [Header("Rune Renderers")]
    public SpriteRenderer[] runeGlows;

    [Header("Glow Settings")]
    public Color glowColor = Color.cyan;
    public float glowIntensity = 6f;

    [Header("Activation")]
    public float glowDelay = 0.15f;

    [Header("Special Settings")]
    public bool alwaysGlow = false;

    bool playerInside = false;

    void Update()
{
    if (alwaysGlow) return;

    // 🔥 stop glow if no altar uses
    if (GameManager.Instance.altarUses <= 0)
    {
        playerInside = false;
        SetGlow(false);
        return;
    }
}

    void Start()
    {
        if (alwaysGlow)
        {
            SetGlow(true);
        }
        else
        {
            SetGlow(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (alwaysGlow) return;

        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            StartCoroutine(ActivateRunes());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (alwaysGlow) return;

        if (other.CompareTag("Player"))
        {
            playerInside = false;
            SetGlow(false);
        }
    }

    IEnumerator ActivateRunes()
    {
        for (int i = 0; i < runeGlows.Length; i++)
        {
            if (!playerInside) yield break;

            if (runeGlows[i] != null)
            {
                Color glow = glowColor * glowIntensity;
                glow.a = 1f;
                runeGlows[i].color = glow;
            }

            yield return new WaitForSeconds(glowDelay);
        }
    }

    void SetGlow(bool state)
    {
        foreach (SpriteRenderer glow in runeGlows)
        {
            if (glow == null) continue;

            if (state)
            {
                Color c = glowColor * glowIntensity;
                c.a = 1f;
                glow.color = c;
            }
            else
            {
                glow.color = new Color(0,0,0,0);
            }
        }
    }
}