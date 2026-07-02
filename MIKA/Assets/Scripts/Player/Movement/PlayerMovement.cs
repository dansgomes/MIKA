using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public event Action OnJumped;
    public event Action OnLanded;

    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public float crouchSpeedDivider = 2f;
    public float runSpeedMultiplier = 1.6f;
    public float horizontalMovement;
    public float verticalInput;

    [Header("Run")]
    public bool isRunning;
    private bool runInputHeld;

    [Header("State")]
    public bool isWalking;
    public bool isMovingHorizontally;
    public bool isJumping;
    public bool isFalling;
    public bool jumpedWhileWalking;
    public bool isLanding;
    public float landingDuration = 0.15f;
    private bool wasAirborne;

    [Header("External Control")]
    public bool isKnockedBack;
    public bool flipLocked;

    [Header("Crouch")]
    public SpriteRenderer spriteRenderer;
    public float crouchOpacity = 0.5f;
    public bool isCrouching;
    public BoxCollider2D capsuleCollider;
    public float crouchColliderHeight = 0.5f;
    private float standingColliderHeight;
    private Vector2 standingColliderOffset;

    [Header("Acceleration")]
    public float accelerationTime = 0f;
    private float velocityXSmoothing;
    private float currentVelocityX;

    [Header("Jump")]
    public float jumpPower = 10f;
    public bool isFacingRight = true;
    bool canJumpNow;

    public Vector2 FacingDirection => new Vector2(isFacingRight ? 1f : -1f, 0f);

    private float lastInteractionDirection = 1f;
    public float InteractionDirection
    {
        get
        {
            if (Mathf.Abs(horizontalMovement) > 0.01f)
            {
                float facingSign = isFacingRight ? 1f : -1f;
                float inputSign = Mathf.Sign(horizontalMovement);
                lastInteractionDirection = facingSign * inputSign;
            }

            return lastInteractionDirection;
        }
    }

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public LayerMask platformLayer;
    public bool inGround;
    public bool onPlatform;
    public bool canJump;

    [Header("Gravity")]
    public float baseGravity = 2;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("Check Walls")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("Ledge Grab")]
    public LayerMask ledgeLayer;
    public Transform ledgeWallCheckPos;
    public Transform ledgeTopCheckPos;
    public float ledgeCheckDistance = 0.5f;
    public Vector2 ledgeClimbOffset = new Vector2(0.3f, 1f);
    public bool isLedgeGrabbed;
    public bool isClimbingLedge;
    public float climbDuration = 0.3f;
    Vector2 ledgeGrabPosition;

    [Header("Ledge Grab")]
    public LayerMask crouchZoneLayer;
    public int crouchZoneCount = 0;
    public bool isInCrouchZone => crouchZoneCount > 0;
    private bool crouchInputHeld;

    private void Awake()
    {
        if (capsuleCollider != null)
        {
            standingColliderHeight = capsuleCollider.size.y;
            standingColliderOffset = capsuleCollider.offset;
        }
    }

    private void Update()
    {
        GroundCheck();

        isMovingHorizontally = Mathf.Abs(horizontalMovement) > 0.01f;

        isWalking = isMovingHorizontally && (inGround || onPlatform);

        isRunning = runInputHeld && isWalking && !isInCrouchZone;

        bool grounded = inGround || onPlatform;

        if (isJumping && grounded && rb.linearVelocity.y <= 0.01f)
        {
            isJumping = false;
            jumpedWhileWalking = false;
        }

        isFalling = !isJumping && !grounded && !isLedgeGrabbed && rb.linearVelocity.y < -0.01f;

        if (isLedgeGrabbed || isClimbingLedge)
        {
            wasAirborne = false;
        }
        else if (!grounded)
        {
            wasAirborne = true;
        }
        else if (wasAirborne)
        {
            wasAirborne = false;
            StartCoroutine(LandingRoutine());
            OnLanded?.Invoke();
        }

        if (isKnockedBack)
        {
            return;
        }

        if (isCrouching && (!inGround || isRunning) && !isInCrouchZone)
        {
            SetCrouching(false);
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

        float targetSpeed = moveSpeed;
        if (isRunning)
        {
            targetSpeed *= runSpeedMultiplier;
        }
        else if (isCrouching)
        {
            targetSpeed /= crouchSpeedDivider;
        }
        currentVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, horizontalMovement * targetSpeed, ref velocityXSmoothing, accelerationTime);
        rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

        Flip();
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private bool CheckForLedge()
    {
        if (inGround) return false;

        Vector2 facingDirection = new Vector2(isFacingRight ? 1f : -1f, 0f);

        RaycastHit2D wallHit = Physics2D.Raycast(ledgeWallCheckPos.position, facingDirection, ledgeCheckDistance, ledgeLayer);
        RaycastHit2D topHit = Physics2D.Raycast(ledgeTopCheckPos.position, facingDirection, ledgeCheckDistance, ledgeLayer);

        return wallHit.collider != null && topHit.collider == null;
    }

    private void GrabLedge()
    {
        isLedgeGrabbed = true;
        isJumping = false;
        isFalling = false;
        jumpedWhileWalking = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        ledgeGrabPosition = transform.position;
    }

    private void HandleLedgeGrabbedInput()
    {
        rb.linearVelocity = Vector2.zero;

        if (verticalInput > 0.1f)
        {
            ClimbLedge();
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

    private void ClimbLedge()
    {
        isLedgeGrabbed = false;

        float facingSign = isFacingRight ? 1f : -1f;
        Vector2 targetPosition = ledgeGrabPosition + new Vector2(ledgeClimbOffset.x * facingSign, ledgeClimbOffset.y);

        transform.position = targetPosition;
        rb.gravityScale = baseGravity;

        StartCoroutine(ClimbLedgeRoutine());
    }

    private IEnumerator ClimbLedgeRoutine()
    {
        isClimbingLedge = true;
        yield return new WaitForSeconds(climbDuration);
        isClimbingLedge = false;
    }

    private IEnumerator LandingRoutine()
    {
        isLanding = true;
        yield return new WaitForSeconds(landingDuration);
        isLanding = false;
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

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontalMovement = input.x;
        verticalInput = input.y;
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
            runInputHeld = true;
        else if (context.canceled)
            runInputHeld = false;
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
            crouchInputHeld = true;
        else if (context.canceled)
            crouchInputHeld = false;

        if (isInCrouchZone) return;
        if (isRunning || !inGround) return;

        if (context.performed)
            SetCrouching(true);
        else if (context.canceled)
            SetCrouching(false);
    }

    private void SetCrouching(bool value)
    {
        isCrouching = value;

        if (capsuleCollider != null)
        {
            if (isCrouching)
            {
                float heightDiff = standingColliderHeight - crouchColliderHeight;
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, crouchColliderHeight);
                capsuleCollider.offset = new Vector2(standingColliderOffset.x, standingColliderOffset.y - heightDiff / 2f);
            }
            else
            {
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, standingColliderHeight);
                capsuleCollider.offset = standingColliderOffset;
            }
        }

        if (spriteRenderer == null) return;

        Color color = spriteRenderer.color;
        color.a = isCrouching ? crouchOpacity : 1f;
        spriteRenderer.color = color;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isInCrouchZone) return;

        if (context.performed)
        {
            if (isLedgeGrabbed) return;

            if (canJumpNow && (inGround || onPlatform || canJump))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                canJumpNow = false;
                isJumping = true;
                jumpedWhileWalking = isWalking;
                OnJumped?.Invoke();
            }
        }
        else if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    //O Jogo (só pra saber se tu tá esperto msm kkkkk)

    private void GroundCheck()
    {
        inGround = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);

        onPlatform = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, platformLayer);

        canJump = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, wallLayer);

        if ((inGround || onPlatform || canJump) && rb.linearVelocity.y <= 0.01f)
        {
            canJumpNow = true;
        }
    }

    public void Flip()
    {
        if (flipLocked) return;

        if ((isFacingRight && horizontalMovement < 0) || (!isFacingRight && horizontalMovement > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(((1 << other.gameObject.layer) & crouchZoneLayer) != 0)
        {
            crouchZoneCount++;
            SetCrouching(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(((1 << other.gameObject.layer) & crouchZoneLayer) != 0)
        {
            crouchZoneCount = Mathf.Max(0, crouchZoneCount - 1);

            if (crouchZoneCount == 0 && !crouchInputHeld)
            {
                SetCrouching(false);
            }
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