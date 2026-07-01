using UnityEngine;

public class BridgeHalf : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public Transform targetPosition;

    private bool isMoving = false;
    private bool hasArrived = false;

    void Update()
    {
        if (!isMoving || hasArrived) return;

        Vector2 current = transform.position;
        Vector2 target = targetPosition.position;

        transform.position = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.01f)
        {
            transform.position = target;
            hasArrived = true;
            isMoving = false;
        }
    }

    public void Activate()
    {
        if (hasArrived) return;
        isMoving = true;
    }

    public bool HasArrived => hasArrived;
}