using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement movement;
    public PlayerDamage damage;
    public PlayerInteractable interactable;
    public float animMoveX;

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
        animator.SetBool("IsRunning", movement.isRunning);
        animator.SetBool("IsJumping", movement.isJumping);
        animator.SetBool("IsFalling", movement.isFalling);
        animator.SetBool("IsCrouching", movement.isCrouching);
        animator.SetBool("IsLedgeGrabbing", movement.isLedgeGrabbed);
        animator.SetBool("IsClimbingLedge", movement.isClimbingLedge);
        animator.SetBool("IsTakingDamage", damage.IsTakingDamage);
        animator.SetBool("IsInteracting", interactable.IsInteracting);
        animator.SetFloat("InteractDirection", movement.InteractionDirection);

        animator.SetFloat("MoveXAbs", Mathf.Abs(movement.horizontalMovement));
    }
}