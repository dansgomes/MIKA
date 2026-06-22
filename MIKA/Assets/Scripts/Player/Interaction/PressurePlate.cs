using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Elevador")]
    public ElevatorInteract targetElevator;

    [Header("Detection")]
    public float restVelocityThreshold = 0.05f;

    private bool isTriggered = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (isTriggered) return;

        BoxInteract box = other.GetComponent<BoxInteract>();
        if (box == null) return;

        if (box.IsBeingInteracted) return;

        Rigidbody2D boxRb = other.GetComponent<Rigidbody2D>();
        if (boxRb == null) return;

        if (boxRb.linearVelocity.sqrMagnitude > restVelocityThreshold * restVelocityThreshold) return;

        TriggerPlate(box);
    }

    private void TriggerPlate(BoxInteract box)
    {
        isTriggered = true;

        box.Lock();

        if (targetElevator != null)
        {
            targetElevator.SetActive(true);
        }
    }
}