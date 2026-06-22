using UnityEngine;

public class ElevatorPlayerDetector : MonoBehaviour
{
    [Header("Referência")]
    public ElevatorInteract elevator;

    [Header("Filtro")]
    public string playerTag = "Player";

    private void Reset()
    {
        if (elevator == null)
        {
            elevator = GetComponentInParent<ElevatorInteract>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (elevator == null) return;

        elevator.OnPlayerEnter(other.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (elevator == null) return;

        elevator.OnPlayerExit(other.transform);
    }
}