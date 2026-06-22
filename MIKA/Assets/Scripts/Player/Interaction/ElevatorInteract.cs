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

    //transform do player atualmente em cima da plataforma (null se nenhum)
    private Transform passenger;
    //parent original do player, restaurado ao sair da plataforma
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

    //chamado pelo ElevatorPlayerDetector (trigger filho) quando o player entra na área de embarque
    public void OnPlayerEnter(Transform player)
    {
        if (passenger != null) return; //já tem um passageiro, ignora

        passenger = player;
        passengerOriginalParent = player.parent;
        player.SetParent(transform, true);

        //zera a velocidade vertical do player no instante do embarque,
        //evitando que uma velocidade de queda residual cause um solavanco
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);
        }
    }

    //chamado pelo ElevatorPlayerDetector (trigger filho) quando o player sai da área de embarque
    public void OnPlayerExit(Transform player)
    {
        if (passenger != player) return; //não é o passageiro atual, ignora

        player.SetParent(passengerOriginalParent, true);
        passenger = null;
        passengerOriginalParent = null;
    }
}