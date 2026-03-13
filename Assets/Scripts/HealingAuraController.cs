using UnityEngine;
using System.Collections;

public class HealingAuraController : MonoBehaviour
{
    [Header("Healing Aura")]
    public ParticleSystem healingAura;

    [Header("Delay Settings")]
    public float auraDelay = 1f;

    bool playerInside = false;

    void Start()
    {
        if (healingAura != null)
            healingAura.Stop();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            StartCoroutine(StartHealingAura());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (healingAura != null)
                healingAura.Stop();
        }
    }

    IEnumerator StartHealingAura()
    {
        yield return new WaitForSeconds(auraDelay);

        if (playerInside && healingAura != null)
            healingAura.Play();
    }
}