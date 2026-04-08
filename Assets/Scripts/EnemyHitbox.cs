using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public EnemyController enemy;

    void Awake()
    {
        if (enemy == null)
            enemy = GetComponentInParent<EnemyController>();
    }

    public void TakeDamage(float damage)
    {
        if (enemy != null && !enemy.IsDead)
        {
            enemy.TakeDamage(damage);
        }
    }
}