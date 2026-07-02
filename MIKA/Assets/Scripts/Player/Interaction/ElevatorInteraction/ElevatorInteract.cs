using UnityEngine;
using System.Collections;

public class ElevatorInteract : MonoBehaviour, IInteractable
{
    public Transform[] stops;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Time Pause")]
    public float waitTimeAtStop = 1f;

    [Header("Control")]
    public bool isActive = true;

    private int targetIndex = 0;
    private Coroutine loopRoutine;

    private Transform passenger;
    private Transform passengerOriginalParent;

    public bool IsBeingInteracted => isActive;
    public bool HoldsPlayer => false;

    void Start()
    {
        if (isActive)
        {
            StartLoop();
        }
    }

    public void Interact(GameObject interactor)
    {
        SetActive(!isActive);
    }

    public void EndInteraction()
    {
    }

    public void SetActive(bool value)
    {
        isActive = value;

        if (isActive)
        {
            StartLoop();
        }
        else
        {
            StopLoop();
        }
    }

    private void StartLoop()
    {
        if (loopRoutine != null) return;
        loopRoutine = StartCoroutine(LoopRoutine());
    }

    private void StopLoop()
    {
        if (loopRoutine == null) return;
        StopCoroutine(loopRoutine);
        loopRoutine = null;
    }

    private IEnumerator LoopRoutine()
    {
        while (true)
        {
            Vector3 targetPosition = stops[targetIndex].position;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;

            yield return new WaitForSeconds(waitTimeAtStop);

            targetIndex = (targetIndex + 1) % stops.Length;
        }
    }

    public void OnPlayerEnter(Transform player)
    {
        if (passenger != null) return;

        passenger = player;
        passengerOriginalParent = player.parent;
        player.SetParent(transform, true);

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);
        }
    }

    public void OnPlayerExit(Transform player)
    {
        if (passenger != player) return;

        player.SetParent(passengerOriginalParent, true);
        passenger = null;
        passengerOriginalParent = null;
    }
}