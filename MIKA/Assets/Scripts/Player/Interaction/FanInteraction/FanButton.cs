using UnityEngine;

public class FanButton : MonoBehaviour, IInteractable
{
    [Header("Target")]
    public FanController targetFan;

    public bool HoldsPlayer => false;

    private bool isUsed = false;

    public bool IsBeingInteracted => isUsed;

    public void Interact(GameObject interactor)
    {
        if (isUsed) return;
        if (targetFan == null) return;

        isUsed = true;
        targetFan.Deactivate();
    }

    public void EndInteraction() { }
}