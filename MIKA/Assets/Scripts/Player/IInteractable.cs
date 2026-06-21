using UnityEngine;

public interface IInteractable
{
    //chamado quando o player aperta o botão de interagir, olhando pra esse objeto
    void Interact(GameObject interactor);

    //chamado quando o player solta a interação
    void EndInteraction();

    //indica se esse objeto já está ocupado
    bool IsBeingInteracted { get; }
}