using UnityEngine;

// placa de pressão dedicada à ativação de uma ponte.
// mesma lógica da PressurePlate de elevador, mas aponta pro BridgeController
// em vez do ElevatorInteract — mantendo as responsabilidades separadas.
public class BridgePressurePlate : MonoBehaviour
{
    [Header("Bridge")]
    public BridgeController targetBridge;

    [Header("Detection")]
    public float restVelocityThreshold = 0.05f;

    private bool isTriggered = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (isTriggered) return;

        BoxInteract box = other.GetComponent<BoxInteract>();
        if (box == null) return;

        // só ativa quando a caixa não está sendo carregada pelo player
        if (box.IsBeingInteracted) return;

        Rigidbody2D boxRb = other.GetComponent<Rigidbody2D>();
        if (boxRb == null) return;

        // só ativa quando a caixa está em repouso
        if (boxRb.linearVelocity.sqrMagnitude > restVelocityThreshold * restVelocityThreshold) return;

        TriggerPlate(box);
    }

    private void TriggerPlate(BoxInteract box)
    {
        isTriggered = true;

        box.Lock();

        if (targetBridge != null)
        {
            targetBridge.SetActive(true);
        }
    }
}