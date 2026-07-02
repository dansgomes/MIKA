using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable
{
    [Header("Setup")]
    public bool oneShot = true;
    private bool hasBeenPulled = false;

    [Header("Events")]
    public UnityEvent onLeverPulled;

    private PlayerAnimator playerAnimator;
    private PlayerMovement playerMovement;

    public bool IsBeingInteracted { get; private set; }
    public bool HoldsPlayer => true;

    public void Interact(GameObject interactor)
    {
        if (oneShot && hasBeenPulled) return;
        if (IsBeingInteracted) return;

        playerAnimator = interactor.GetComponent<PlayerAnimator>();
        playerMovement = interactor.GetComponent<PlayerMovement>();

        IsBeingInteracted = true;
        hasBeenPulled = true;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerAnimator != null)
            playerAnimator.SetPullingLever(true);
    }

    public void EndInteraction()
    {
        IsBeingInteracted = false;
        onLeverPulled?.Invoke();
    }
}