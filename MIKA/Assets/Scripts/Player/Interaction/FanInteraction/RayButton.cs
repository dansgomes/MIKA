using UnityEngine;

public class RayButton : MonoBehaviour, IInteractable
{
    [Header("Target")]
    public RayController targetFan;

    public bool HoldsPlayer => false;

    private bool isUsed = false;

    public Animator eletricityAnimator;
    public SpriteRenderer eletricityRenderer;

    public bool IsBeingInteracted => isUsed;

    public void Interact(GameObject interactor)
    {
        if (isUsed) return;
        if (targetFan == null) return;

        isUsed = true;
        targetFan.Deactivate();
        eletricityAnimator.enabled = false;
        eletricityRenderer.enabled = false;
    }

    public void EndInteraction() { }
}