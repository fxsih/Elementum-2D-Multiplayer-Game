using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.5f;

    [Header("Dash FX")]
    public GameObject dashFxPrefab;

    [Header("Jump (Visual Only)")]
    public float jumpHeight = 0.55f;
    public float jumpDuration = 0.22f;
    public AnimationCurve jumpCurve;

    public float downJumpForce = 8f;     // Increased to clear 2 blocks
public float downJumpDuration = 0.8f; // Longer air-time for the drop
private float currentJumpDuration;    // Tracks which duration we are using
public float slideSpeed = 5f; // How fast to slide off the wall

    [Header("Dash Jump Combo")]
    public float dashJumpForwardBoost = 7f;
    public float dashJumpHeightMultiplier = 1.4f;

    [Header("Air Control")]
    public float airControl = 0.6f;

    [Header("High Ground Area")]
    public Collider2D highGroundArea;

    [Header("Shadow")]
    public Vector2 shadowMinScale = new Vector2(0.25f, 0.15f);
    public float shadowLeftOffsetX = -0.01f;

    [Header("Run Dust FX")]
    public GameObject runDustPrefab;
    public Vector2 runDustOffsetRight = Vector2.zero;
    public Vector2 runDustOffsetLeft = new Vector2(-0.5f, 0f);

    Rigidbody2D rb;
    Collider2D playerCollider;
    SpriteRenderer sprite;
    Animator animator;

    Transform visual;
    Transform shadow;

    GameObject activeRunDust;

    bool wasRunning;

    PlayerInputActions input;

    Vector2 moveInput;
    Vector2 mouseScreenPos;
    Vector2 dashDirection;

    bool isJumping;
    bool wasHighGroundJump = false; // Tracks if the current jump started on high ground
    bool isDashing;
    bool dashLocked;

    float dashLockTimer;
    float jumpTimer;
    float dashTimer;

    Vector3 visualBasePos;
    Vector3 shadowBasePos;
    Vector3 shadowBaseScale;

    Vector2 dashJumpMomentum;

    bool jumpQueued;
    float jumpBufferTimer;
    float jumpBufferTime = 0.55f;

    List<Collider2D> ignoredWalls = new List<Collider2D>();

    int playerLayer;
    int jumpBlockLayer;

    public bool IsJumping => isJumping;
    public bool IsDashing => isDashing;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        sprite = animator.GetComponent<SpriteRenderer>();

        visual = animator.transform.parent;
        shadow = transform.Find("Shadow");

        visualBasePos = visual.localPosition;
        shadowBasePos = shadow.localPosition;
        shadowBaseScale = shadow.localScale;

        playerLayer = LayerMask.NameToLayer("Player");
        jumpBlockLayer = LayerMask.NameToLayer("JumpBlock");
    }

    void OnEnable()
    {
        input = new PlayerInputActions();
        input.Gameplay.Enable();

        input.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Gameplay.Move.canceled += _ => moveInput = Vector2.zero;

        input.Gameplay.MousePosition.performed += ctx => mouseScreenPos = ctx.ReadValue<Vector2>();

        input.Gameplay.Jump.performed += _ => QueueJump();
        input.Gameplay.Dash.performed += _ => TryStartDash();
    }

    void OnDisable()
    {
        input.Gameplay.Disable();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else if (isJumping)
        {
            Vector2 airMove = moveInput * moveSpeed * airControl;

            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                dashJumpMomentum + airMove,
                0.2f
            );
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    void Update()
    {
        if (jumpQueued)
        {
            jumpBufferTimer -= Time.deltaTime;
            if (jumpBufferTimer <= 0f)
                jumpQueued = false;
        }

        UpdateFacing();
        UpdateAnimation();
        UpdateJump();
        UpdateShadowOffset();
        UpdateRunDust();
        UpdateDash();
        UpdateDashCooldown();
    }

    void TryStartDash()
    {
        if (isDashing) return;
        if (dashLocked) return;

        StartDash();
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = 0f;

        if (moveInput.sqrMagnitude > 0.01f)
            dashDirection = moveInput.normalized;
        else
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f)
            );

            dashDirection = (mouseWorld - transform.position).normalized;
        }

        visual.gameObject.SetActive(false);
        DestroyRunDust();

        SpawnDashFX();
    }

    void UpdateDash()
    {
        if (!isDashing) return;

        dashTimer += Time.deltaTime;

        if (dashTimer >= dashDuration)
        {
            isDashing = false;

            if (!isJumping)
                rb.linearVelocity = Vector2.zero;

            visual.gameObject.SetActive(true);

            dashLocked = true;
            dashLockTimer = 0f;

            if (jumpQueued)
            {
                jumpQueued = false;
                StartJump();
            }
        }
    }

    void UpdateDashCooldown()
    {
        if (!dashLocked) return;

        dashLockTimer += Time.deltaTime;

        if (dashLockTimer >= dashCooldown)
            dashLocked = false;
    }

    void SpawnDashFX()
    {
        if (!dashFxPrefab) return;

        float angle =
            Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg + 180f;

        GameObject fx = Instantiate(
            dashFxPrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, angle),
            transform
        );

        StartCoroutine(UpdateFxSorting(fx));

        Destroy(fx, dashDuration);
    }

    IEnumerator UpdateFxSorting(GameObject fx)
    {
        SpriteRenderer playerRenderer = sprite;

        SpriteRenderer[] srs = fx.GetComponentsInChildren<SpriteRenderer>();
        ParticleSystemRenderer[] ps = fx.GetComponentsInChildren<ParticleSystemRenderer>();

        while (fx != null)
        {
            int order = playerRenderer.sortingOrder - 1;

            foreach (SpriteRenderer r in srs)
            {
                r.sortingLayerName = playerRenderer.sortingLayerName;
                r.sortingOrder = order;
            }

            foreach (ParticleSystemRenderer p in ps)
            {
                p.sortingLayerName = playerRenderer.sortingLayerName;
                p.sortingOrder = order;
            }

            yield return null;
        }
    }

    void QueueJump()
    {
        if (!isDashing)
        {
            StartJump();
            return;
        }

        jumpQueued = true;
        jumpBufferTimer = jumpBufferTime;
    }

    void UpdateFacing()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f)
        );

        sprite.flipX = mouseWorld.x < transform.position.x;
    }

    void UpdateAnimation()
    {
        animator.SetFloat("Speed", rb.linearVelocity.sqrMagnitude);
    }

  void StartJump()
{
    if (isJumping) return;

    bool insideHighGround = (highGroundArea != null && highGroundArea.IsTouching(playerCollider));
    wasHighGroundJump = insideHighGround;

    // Default duration
    currentJumpDuration = jumpDuration;

    if (insideHighGround)
    {
        Physics2D.IgnoreLayerCollision(playerLayer, jumpBlockLayer, true);

        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (moveInput.sqrMagnitude > 0.01f) 
        {
            // USE STRONGER PUSH: downJumpForce instead of 3f
            rb.linearVelocity = moveInput.normalized * downJumpForce;
        }

        // USE LONGER DURATION: to give them time to fly over the 2 blocks
        currentJumpDuration = downJumpDuration;
        
        dashJumpMomentum = Vector2.zero; 
        isJumping = true;
        jumpTimer = 0f;
        animator.SetTrigger("Jump");
        return; 
    }

    // Normal jump logic...
    isJumping = true;
    jumpTimer = 0f;
    dashJumpMomentum = rb.linearVelocity;

    if (isDashing)
    {
        dashJumpMomentum = Vector2.Lerp(
            rb.linearVelocity,
            dashDirection * dashJumpForwardBoost,
            0.5f
        );
    }

    animator.SetTrigger("Jump");
}
    void UpdateJump()
{
    if (!isJumping) return;

    jumpTimer += Time.deltaTime;
    // USE THE DYNAMIC DURATION
    float t = Mathf.Clamp01(jumpTimer / currentJumpDuration);

    float heightMultiplier = wasHighGroundJump ? dashJumpHeightMultiplier : 1f;
    float h = jumpCurve.Evaluate(t) * heightMultiplier;

    visual.localPosition = visualBasePos + Vector3.up * (h * jumpHeight);
    shadow.localScale = Vector3.Lerp(shadowBaseScale, shadowMinScale, h);

    if (t >= 1f)
{
    isJumping = false;
    visual.localPosition = visualBasePos;
    shadow.localScale = shadowBaseScale;

    // Pass the direction we were moving so we know which way to slide
    Vector2 slideDir = rb.linearVelocity.normalized;
    StartCoroutine(RestoreWallCollision(slideDir));
}
}

    IEnumerator RestoreWallCollision(Vector2 slideDirection)
{
    // 1. Check if we are currently overlapping a wall
    // This uses the player's collider to see if it's hitting the jumpBlockLayer
    ContactFilter2D filter = new ContactFilter2D();
    filter.SetLayerMask(jumpBlockLayer);
    filter.useTriggers = false;

    Collider2D[] results = new Collider2D[1];
    
    // 2. While the player is still "inside" the wall, keep sliding
    while (playerCollider.Overlap(filter, results) > 0)
    {
        // Keep collisions off so we don't get stuck/pop up
        Physics2D.IgnoreLayerCollision(playerLayer, jumpBlockLayer, true);
        
        // Slide the player in the direction they were jumping
        rb.linearVelocity = slideDirection * slideSpeed;
        
        yield return null; // Wait for next frame
    }

    // 3. Once clear of the wall, restore normal physics
    Physics2D.IgnoreLayerCollision(playerLayer, jumpBlockLayer, false);
}

    void UpdateShadowOffset()
    {
        shadow.localPosition = shadowBasePos;

        if (sprite.flipX)
        {
            shadow.localPosition = new Vector3(
                shadowBasePos.x + shadowLeftOffsetX,
                shadowBasePos.y,
                shadowBasePos.z
            );
        }
    }

    void UpdateRunDust()
    {
        bool isActuallyMoving =
            rb.linearVelocity.sqrMagnitude > 0.01f &&
            !isJumping &&
            !isDashing;

        if (isActuallyMoving && !wasRunning)
            SpawnRunDust();

        if (!isActuallyMoving && wasRunning)
            DestroyRunDust();

        wasRunning = isActuallyMoving;

        if (!activeRunDust) return;

        Vector2 offset = sprite.flipX ? runDustOffsetLeft : runDustOffsetRight;

        activeRunDust.transform.localPosition = offset;

        SpriteRenderer sr = activeRunDust.GetComponentInChildren<SpriteRenderer>();

        if (sr) sr.flipX = sprite.flipX;
    }

    void SpawnRunDust()
    {
        if (!runDustPrefab || activeRunDust) return;

        activeRunDust = Instantiate(runDustPrefab, transform);

        StartCoroutine(UpdateFxSorting(activeRunDust));
    }

    void DestroyRunDust()
    {
        if (!activeRunDust) return;

        Destroy(activeRunDust);
        activeRunDust = null;
    }
}