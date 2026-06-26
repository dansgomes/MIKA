using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    public Animator animator;
    public PlayerMovement movement;
    public float animMoveX;

    void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        animator.SetFloat("MoveX", movement.horizontalMovement);

        animator.SetBool("IsWalking", movement.isWalking);

        animator.SetBool("IsRunning", movement.isRunning);

        animator.SetBool("IsJumping", movement.isJumping);

        animator.SetBool("IsCrouching", movement.isCrouching);

        animator.SetBool("IsLedgeGrabbing", movement.isLedgeGrabbed);

    }
}
