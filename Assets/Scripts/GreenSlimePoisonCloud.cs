using UnityEngine;

public class GreenSlimePoisonCloud : MonoBehaviour
{
    public GameObject poisonCloudPrefab;
    public float detectionRange = 2.0f; // 🔥 bigger than before

    Transform player;
    EnemyController enemy;

    bool triggered = false;

    void Start()
    {
        enemy = GetComponent<EnemyController>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (enemy == null || enemy.IsDead) return;
        if (player == null) return;
        if (triggered) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            TriggerPoison();
        }
    }

    void TriggerPoison()
    {
        triggered = true;

        if (poisonCloudPrefab != null)
        {
            Instantiate(poisonCloudPrefab, transform.position, Quaternion.identity);
        }

        enemy.TakeDamage(25f, false);
    }
}