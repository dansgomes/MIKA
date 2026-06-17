using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    float horizontalMovement;
    float verticalInput;
    private int currentDirection = 0;

    [Header("Run")]
    public float runSpeedMultiplier = 1.6f;
    bool isRunning;

    [Header("External Control")]
    public bool isKnockedBack;

    [Header("Crouch")]
    public SpriteRenderer spriteRenderer;
    public float crouchOpacity = 0.5f;
    bool isCrouching;

    [Header("Acceleration")]
    public float accelerationTime = 0f;
    private float velocityXSmoothing;
    private float currentVelocityX;

    [Header("Jump")]
    public float jumpPower = 10f;
    bool isFacingRight = true;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool inGround;
    public bool canJump;

    [Header("Gravity")]
    public float baseGravity = 2;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("Check Walls")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("Wall Movement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;

    [Header("Wall Jump")]
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.3f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);
    bool canWallJump = false;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public float dashEndSmoothTime = 0.15f;
    public float dashEndSmoothDuration = 0.2f;
    bool isDashing;
    float dashTimer;
    float lastDashTime;
    int lastDirection;
    float dashEndSmoothTimer;

    [Header("Wall Climb")]
    public float wallClimbSpeed = 2f;

    [Header("Ledge Grab")]
    public LayerMask ledgeLayer;
    public Transform ledgeWallCheckPos;
    public Transform ledgeTopCheckPos;
    public float ledgeCheckDistance = 0.5f;
    public Vector2 ledgeClimbOffset = new Vector2(0.3f, 1f);
    public float ledgeClimbDuration = 0.25f;
    bool isLedgeGrabbed;
    bool isLedgeClimbing;
    Vector2 ledgeGrabPosition;

    private void Update()
    {
        GroundCheck();

        if (isKnockedBack)
        {
            return;
        }

        if (isCrouching && (!inGround || isRunning))
        {
            SetCrouching(false);
        }

        if (isLedgeClimbing)
        {
            return;
        }

        if (isLedgeGrabbed)
        {
            HandleLedgeGrabbedInput();
            return;
        }

        if (!inGround && rb.linearVelocity.y <= 0f && CheckForLedge())
        {
            GrabLedge();
            return;
        }

        Gravity();

        ProcessWallSlide();
        ProcessWallJump();

        if (isWallSliding && verticalInput > 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallClimbSpeed);
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashEndSmoothTimer = dashEndSmoothDuration;
            }
            else
            {
                rb.linearVelocity = new Vector2(lastDirection * dashSpeed, 0f);
                GroundCheck();
                return;
            }
        }

        if (!isWallJumping && !isDashing)
        {
            float smoothTime = accelerationTime;

            if (dashEndSmoothTimer > 0f)
            {
                dashEndSmoothTimer -= Time.deltaTime;
                smoothTime = dashEndSmoothTime;
            }

            float targetSpeed = moveSpeed * (isRunning ? runSpeedMultiplier : 1f);
            currentVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, horizontalMovement * targetSpeed, ref velocityXSmoothing, smoothTime);
            rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

            Flip();
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private bool CheckForLedge()
    {
        Vector2 facingDirection = new Vector2(isFacingRight ? 1f : -1f, 0f);

        RaycastHit2D wallHit = Physics2D.Raycast(ledgeWallCheckPos.position, facingDirection, ledgeCheckDistance, ledgeLayer);
        RaycastHit2D topHit = Physics2D.Raycast(ledgeTopCheckPos.position, facingDirection, ledgeCheckDistance, ledgeLayer);

        return wallHit.collider != null && topHit.collider == null;
    }

    private void GrabLedge()
    {
        isLedgeGrabbed = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        ledgeGrabPosition = transform.position;
    }

    private void HandleLedgeGrabbedInput()
    {
        rb.linearVelocity = Vector2.zero;

        if (verticalInput > 0.1f)
        {
            StartCoroutine(ClimbLedgeRoutine());
        }
        else if (verticalInput < -0.1f)
        {
            DropFromLedge();
        }
    }

    private void DropFromLedge()
    {
        isLedgeGrabbed = false;
        rb.gravityScale = baseGravity;
    }

    private System.Collections.IEnumerator ClimbLedgeRoutine()
    {
        isLedgeGrabbed = false;
        isLedgeClimbing = true;

        float facingSign = isFacingRight ? 1f : -1f;
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = ledgeGrabPosition + new Vector2(ledgeClimbOffset.x * facingSign, ledgeClimbOffset.y);

        float elapsed = 0f;
        while (elapsed < ledgeClimbDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / ledgeClimbDuration);
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        rb.gravityScale = baseGravity;
        isLedgeClimbing = false;
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if (!inGround && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            canWallJump = true;
            wallJumpDirection = -Mathf.Sign(transform.localScale.x);
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0f)
                canWallJump = false;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontalMovement = input.x;
        verticalInput = input.y;

        if (horizontalMovement != 0)
            currentDirection = (int)Mathf.Sign(horizontalMovement);
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
            isRunning = true;
        else if (context.canceled)
            isRunning = false;
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (isRunning || !inGround) return;

        if (context.performed)
            SetCrouching(true);
        else if (context.canceled)
            SetCrouching(false);
    }

    private void SetCrouching(bool value)
    {
        isCrouching = value;

        if (spriteRenderer == null) return;

        Color color = spriteRenderer.color;
        color.a = isCrouching ? crouchOpacity : 1f;
        spriteRenderer.color = color;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isLedgeClimbing) return;

        if (context.performed)
        {
            if (isLedgeGrabbed)
            {
                isLedgeGrabbed = false;
                rb.gravityScale = baseGravity;
                rb.linearVelocity = new Vector2(-(isFacingRight ? 1f : -1f) * wallJumpPower.x, wallJumpPower.y);
                return;
            }

            if (canWallJump)
            {
                isWallJumping = true;
                canWallJump = false;

                rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
                wallJumpTimer = 0f;

                if ((wallJumpDirection > 0 && transform.localScale.x < 0) ||
                    (wallJumpDirection < 0 && transform.localScale.x > 0))
                {
                    isFacingRight = !isFacingRight;
                    Vector3 ls = transform.localScale;
                    ls.x *= -1f;
                    transform.localScale = ls;
                }

                Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
            }
            else if (jumpsRemaining > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
            }
        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (isLedgeGrabbed || isLedgeClimbing) return;

        if (context.performed && !isDashing && Time.time - lastDashTime > dashCooldown)
        {
            if (currentDirection != 0)
            {
                lastDirection = currentDirection;
                isDashing = true;
                dashTimer = dashDuration;
                lastDashTime = Time.time;
                rb.linearVelocity = new Vector2(lastDirection * dashSpeed, 0f);
            }
        }
    }

    private void GroundCheck()
    {
        inGround = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        canJump = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, wallLayer);

        if (inGround || canJump)
        {
            jumpsRemaining = maxJumps;
        }

        if (inGround && isWallJumping)
        {
            CancelInvoke(nameof(CancelWallJump));
            isWallJumping = false;
        }
    }

    private void Flip()
    {
        if ((isFacingRight && horizontalMovement < 0) || (!isFacingRight && horizontalMovement > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);

        if (ledgeWallCheckPos != null)
        {
            Gizmos.color = Color.green;
            Vector2 dir = new Vector2(isFacingRight ? 1f : -1f, 0f) * ledgeCheckDistance;
            Gizmos.DrawLine(ledgeWallCheckPos.position, (Vector2)ledgeWallCheckPos.position + dir);
        }

        if (ledgeTopCheckPos != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 dir = new Vector2(isFacingRight ? 1f : -1f, 0f) * ledgeCheckDistance;
            Gizmos.DrawLine(ledgeTopCheckPos.position, (Vector2)ledgeTopCheckPos.position + dir);
        }
    }
}