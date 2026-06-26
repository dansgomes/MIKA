using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractable : MonoBehaviour
{
    public float distance = 1f;
    public LayerMask interactableMask;

    private InputSystem_Actions playerinput;
    private InputAction interact;

    private PlayerMovement playerMovement;

    private IInteractable currentInteractable;

    public bool IsInteracting => currentInteractable != null;

    void Awake()
    {
        playerinput = new InputSystem_Actions();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        interact = playerinput.player.interact;
        interact.Enable();

        playerMovement.OnJumped += EndCurrentInteraction;
    }

    void OnDisable()
    {
        interact.Disable();

        playerMovement.OnJumped -= EndCurrentInteraction;
    }

    void Start()
    {
        Physics2D.queriesStartInColliders = false;
    }

    void Update()
    {
        if (!interact.WasPressedThisFrame()) return;

        if (currentInteractable == null)
        {
            TryStartInteraction();
        }
        else
        {
            EndCurrentInteraction();
        }
    }

    private void TryStartInteraction()
    {
        Vector2 facingDirection = playerMovement.FacingDirection;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, distance, interactableMask);

        if (hit.collider == null) return;

        IInteractable interactable = hit.collider.GetComponent<IInteractable>();
        if (interactable == null) return;

        interactable.Interact(gameObject);

        if (interactable.HoldsPlayer)
        {
            currentInteractable = interactable;
            playerMovement.flipLocked = true;
        }
    }

    private void EndCurrentInteraction()
    {
        if (currentInteractable == null) return;

        currentInteractable.EndInteraction();
        currentInteractable = null;
        playerMovement.flipLocked = false;
    }

    void OnDrawGizmos()
    {
        if (playerMovement == null) return;
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + playerMovement.FacingDirection * distance);
    }
}