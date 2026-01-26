using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 16f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.4f;

    Rigidbody2D rb;
    PlayerController controller;
    PlayerInputActions input;

    Vector2 dashDirection;
    bool isDashing;
    float dashTimer;
    float cooldownTimer;

    public bool IsDashing => isDashing;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        input = new PlayerInputActions();
        input.Gameplay.Enable();
        input.Gameplay.Dash.performed += _ => TryDash();
    }

    void OnDisable()
    {
        input.Gameplay.Dash.performed -= _ => TryDash();
        input.Gameplay.Disable();
    }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isDashing) return;

        rb.linearVelocity = dashDirection * dashSpeed;

        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)
            EndDash();
    }

    void TryDash()
    {
        if (isDashing) return;
        if (cooldownTimer > 0f) return;

        dashDirection = rb.linearVelocity.normalized;

        // If idle, dash in facing direction
        if (dashDirection == Vector2.zero)
{
    Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    dashDirection = (mouseWorld - transform.position).normalized;
}


        StartDash();
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
    }

    void EndDash()
    {
        isDashing = false;
    }
}
