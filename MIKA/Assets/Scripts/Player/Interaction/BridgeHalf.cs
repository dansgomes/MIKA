using UnityEngine;

// controla uma metade da ponte.
// quando ativada, move em direção à posição alvo até chegar lá e parar.
public class BridgeHalf : MonoBehaviour
{
    [Header("Movement")]
    // velocidade de movimento da metade da ponte
    public float moveSpeed = 2f;
    // posição alvo (onde essa metade deve parar, encostando na outra)
    public Transform targetPosition;

    private bool isMoving = false;
    private bool hasArrived = false;

    void Update()
    {
        if (!isMoving || hasArrived) return;

        Vector2 current = transform.position;
        Vector2 target = targetPosition.position;

        // move em direção ao alvo
        transform.position = Vector2.MoveTowards(current, target, moveSpeed * Time.deltaTime);

        // verifica se chegou (distância menor que um threshold mínimo)
        if (Vector2.Distance(transform.position, target) < 0.01f)
        {
            transform.position = target;
            hasArrived = true;
            isMoving = false;
        }
    }

    // chamado pelo BridgeController quando a ponte deve começar a fechar
    public void Activate()
    {
        if (hasArrived) return;
        isMoving = true;
    }

    public bool HasArrived => hasArrived;
}