using UnityEngine;
using Pathfinding;

public class WitchShooter : MonoBehaviour
{
    public GameObject projectilePrefab;

    public float stopDistance = 4f;
    public float runDistance = 7f;
    public float shootCooldown = 2f;

    public Transform firePoint;

    bool isAttacking = false;
    bool hasFiredThisAttack = false;

    [Header("Respawn Settings")]
public float despawnDistance = 20f;
public float respawnDistance = 12f;

    float shootTimer;

    Transform player;
    EnemyController enemy;
    AIPath ai;
    Animator animator;

    Vector2 GetValidSpawnPosition(Vector2 desiredPos)
{
    NNInfo nearest = AstarPath.active.GetNearest(desiredPos, NNConstraint.Default);

    if (nearest.node != null && nearest.node.Walkable)
    {
        return (Vector2)nearest.position;
    }

    return desiredPos; // fallback (rare)
}

    void Start()
    {
        enemy = GetComponent<EnemyController>();
        ai = GetComponent<AIPath>();
        animator = GetComponent<Animator>(); // ✅ FIXED

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

    float dist = Vector2.Distance(transform.position, player.position);

    if (dist > despawnDistance)
    {
        RespawnNearPlayer();
    }

        if (enemy.IsDead) return;
        
        if (enemy == null || enemy.IsDead) return;

        // 🔥 EXTRA SAFETY
        if (animator != null && animator.GetBool("IsDead")) return;

        if (player == null) return;

        shootTimer -= Time.deltaTime;

        // 🔥 MOVEMENT
        if (dist > runDistance)
        {
            ai.canMove = true;
            ai.maxSpeed = 4.5f;
        }
        else if (dist > stopDistance)
        {
            ai.canMove = true;
            ai.maxSpeed = 2.5f;
        }
        else
        {
            Vector2 dir = (transform.position - player.position).normalized;
            Vector3 holdPosition = player.position + (Vector3)(dir * stopDistance);

            ai.destination = holdPosition;
            ai.maxSpeed = 0.02f;
        }

        // 🔥 SHOOT
        if (dist <= stopDistance && shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }

        // 🔥 ANIMATION
        float speed = ai.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        FacePlayer();
    }

    void Shoot()
    {
        if (isAttacking) return;

        isAttacking = true;
        hasFiredThisAttack = false;

        animator.SetTrigger("Attack");
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void SpawnProjectile()
    {
        if (hasFiredThisAttack) return;

        FireProjectile();
        hasFiredThisAttack = true;
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;

        if (player.position.x < transform.position.x)
            scale.x = -Mathf.Abs(scale.x);
        else
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    void FireProjectile()
    {
        if (player == null) return;

        Collider2D playerCol = player.GetComponent<Collider2D>();

        Vector2 targetPos = playerCol != null
            ? playerCol.bounds.center
            : (Vector2)player.position;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        Vector2 dir = (targetPos - (Vector2)spawnPos).normalized;

        spawnPos += (Vector3)(dir * 0.5f);

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        proj.GetComponent<WitchProjectile>()?.SetDirection(dir);
    }

   void RespawnNearPlayer()
{
    Camera cam = Camera.main;
    if (cam == null) return;

    float camHeight = cam.orthographicSize;
    float camWidth = camHeight * cam.aspect;

    int side = Random.Range(0, 4);
    Vector2 offset = Vector2.zero;

    switch (side)
    {
        case 0: // left
            offset = new Vector2(-camWidth - 2f, Random.Range(-camHeight, camHeight));
            break;

        case 1: // right
            offset = new Vector2(camWidth + 2f, Random.Range(-camHeight, camHeight));
            break;

        case 2: // top
            offset = new Vector2(Random.Range(-camWidth, camWidth), camHeight + 2f);
            break;

        case 3: // bottom
            offset = new Vector2(Random.Range(-camWidth, camWidth), -camHeight - 2f);
            break;
    }

    Vector2 desiredPos = (Vector2)player.position + offset;

    // 🔥 THIS IS THE IMPORTANT PART
   Vector2 validPos = GetSafeSpawnPosition(desiredPos);

    transform.position = validPos;

    // reset AI safely
    if (ai != null)
    {
        ai.Teleport(validPos, true);
    }

    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
        rb.linearVelocity = Vector2.zero;
}

Vector2 GetSafeSpawnPosition(Vector2 basePos)
{
    for (int i = 0; i < 5; i++)
    {
        Vector2 randomOffset = Random.insideUnitCircle * 2f;
        Vector2 testPos = basePos + randomOffset;

        NNInfo nearest = AstarPath.active.GetNearest(testPos, NNConstraint.Default);

        if (nearest.node != null && nearest.node.Walkable)
        {
            return (Vector2)nearest.position;
        }
    }

    return basePos; // fallback
}


}