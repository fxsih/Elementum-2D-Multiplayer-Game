using UnityEngine;

public class JumpDustController : MonoBehaviour
{
    [Header("Jump Dust")]
    public GameObject jumpDustPrefab;
    public Vector3 dustOffset = new Vector3(0f, -0.1f, 0f);

    PlayerController player;
    bool wasJumping;

    void Awake()
    {
        player = GetComponent<PlayerController>();
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

        Instantiate(
            jumpDustPrefab,
            transform.position + dustOffset,
            Quaternion.identity
        );
    }
}
