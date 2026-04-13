using UnityEngine;
using System.Collections;

public class FireTrap : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 3f;
    public float burnDuration = 5f;
    public float hitRadius = 1.2f;
    public float cooldown = 3f;

    [Header("References")]
    public Transform hitPoint;

    bool isActive = true;

    Animator anim;
    Collider2D col;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.GetComponentInParent<PlayerController>() != null)
        {
            isActive = false;
            anim.SetTrigger("Activate");
            StartCoroutine(CooldownRoutine());
        }
    }

    // 🔥 CALLED FROM ANIMATION EVENT
    public void OnTrapHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRadius);

        foreach (Collider2D hit in hits)
        {
            PlayerController player = hit.GetComponentInParent<PlayerController>();

            if (player != null)
            {
                StartCoroutine(ApplyBurn(player));
            }
        }
    }

   IEnumerator ApplyBurn(PlayerController player)
{
    if (player != null)
    {
        // 🔥 APPLY BURN EFFECT ONCE
        player.ApplyBurn(damagePerSecond, burnDuration);
    }

    float timer = 0f;

    while (timer < burnDuration)
    {
        if (player != null)
        {
            Vector2 dir = (player.transform.position - hitPoint.position).normalized;
            player.TakeDamage(damagePerSecond, dir);
        }

        yield return new WaitForSeconds(1f);
        timer += 1f;
    }
}

    IEnumerator CooldownRoutine()
    {
        col.enabled = false;

        yield return new WaitForSeconds(cooldown);

        col.enabled = true;
        isActive = true;
    }

    void OnDrawGizmosSelected()
    {
        if (hitPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(hitPoint.position, hitRadius);
    }
}