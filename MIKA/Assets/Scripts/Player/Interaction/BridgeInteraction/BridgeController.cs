using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [Header("Bridge Halves")]
    public BridgeHalf leftHalf;
    public BridgeHalf rightHalf;

    private bool isActive = false;

    public void SetActive(bool value)
    {
        if (isActive) return;

        isActive = value;

        if (isActive)
        {
            if (leftHalf != null) leftHalf.Activate();
            if (rightHalf != null) rightHalf.Activate();
        }
    }

    public bool IsComplete => 
        (leftHalf == null || leftHalf.HasArrived) && 
        (rightHalf == null || rightHalf.HasArrived);
}