using UnityEngine;

public class JumpDustController : MonoBehaviour
{
    [Header("Jump Dust")]
    public GameObject jumpDustPrefab;
    public Vector3 dustOffset = new Vector3(0f, -0.1f, 0f);

    PlayerController player;
    bool wasJumping;

    SpriteRenderer playerRenderer;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        playerRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!player) return;

        bool isJumping = player.IsJumping;

        // Detect jump START
        if (!wasJumping && isJumping)
        {
            SpawnJumpDust();
        }

        wasJumping = isJumping;
    }

  void SpawnJumpDust()
{
    if (!jumpDustPrefab) return;

    GameObject dust = Instantiate(
        jumpDustPrefab,
        transform.position + dustOffset,
        Quaternion.identity
    );

    int order = Mathf.RoundToInt(-dust.transform.position.y * 100) - 1;

    // Fix ALL sprite renderers inside the effect
    SpriteRenderer[] renderers = dust.GetComponentsInChildren<SpriteRenderer>();

    foreach (SpriteRenderer r in renderers)
    {
        r.sortingLayerName = "Player";
        r.sortingOrder = order;
    }
}
}