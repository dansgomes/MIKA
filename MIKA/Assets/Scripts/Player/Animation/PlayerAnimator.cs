using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement movement;
    public PlayerDamage damage;
    public PlayerInteractable interactable;
    public float animMoveX;

    [Header("Visual State")]
    [Tooltip("Override Controller usado depois que a personagem perde a mão (evento narrativo permanente).")]
    public AnimatorOverrideController oneHandOverride;
    private bool hasOneHand;

    void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        damage = GetComponent<PlayerDamage>();
        interactable = GetComponent<PlayerInteractable>();
    }

    void Update()
    {
        animator.SetFloat("MoveX", movement.horizontalMovement);
        animator.SetBool("IsWalking", movement.isWalking);
        animator.SetBool("IsMovingHorizontally", movement.isMovingHorizontally);
        animator.SetBool("IsRunning", movement.isRunning);
        animator.SetBool("IsJumping", movement.isJumping);
        animator.SetBool("IsLanding", movement.isLanding);
        animator.SetBool("IsFalling", movement.isFalling);
        animator.SetBool("JumpedWhileWalking", movement.jumpedWhileWalking);
        animator.SetBool("IsCrouching", movement.isCrouching);
        animator.SetBool("IsLedgeGrabbing", movement.isLedgeGrabbed);
        animator.SetBool("IsClimbingLedge", movement.isClimbingLedge);
        animator.SetBool("IsTakingDamage", damage.IsTakingDamage);
        animator.SetBool("IsInteracting", interactable.IsInteracting);
        animator.SetFloat("InteractDirection", movement.InteractionDirection);

        animator.SetFloat("MoveXAbs", Mathf.Abs(movement.horizontalMovement));

        if (Input.GetKeyDown(KeyCode.L))
        {
            SetOneHanded();
        }
    }

    public void SetOneHanded()
    {
        if (hasOneHand) return;
        if (oneHandOverride == null)
        {
            Debug.LogWarning("PlayerAnimator: oneHandOverride não foi atribuído no Inspector.");
            return;
        }

        animator.runtimeAnimatorController = oneHandOverride;
        hasOneHand = true;
    }
}