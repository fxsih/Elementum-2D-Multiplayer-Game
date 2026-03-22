using UnityEngine;

public class AltarHeal : MonoBehaviour
{
    public float healPerSecond = 15f; // healing speed

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            // 🔥 Heal smoothly over time
            player.Heal(healPerSecond * Time.deltaTime);
        }
    }
}