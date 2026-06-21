using UnityEngine;

public enum InteractionTypes
{
    Alone,
    Hand
}

public class BoxInteract : MonoBehaviour, IInteractable
{
    private Rigidbody2D rb;
    private FixedJoint2D joint;

    private InteractionTypes interaction;
    public InteractionTypes Interaction
    {
        get => interaction;
        set
        {
            interaction = value;
            Interactions();
        }
    }

    public bool IsBeingInteracted => interaction == InteractionTypes.Hand;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<FixedJoint2D>();
        Interactions();
    }

    private void Interactions()
    {
        switch (interaction)
        {
            case InteractionTypes.Alone:
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.mass = 15;
                break;

            case InteractionTypes.Hand:
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.mass = 1;
                break;
        }
    }

    //chamado pelo PlayerInteractable ao apertar o botão de interagir
    public void Interact(GameObject interactor)
    {
        joint.enabled = true;
        joint.connectedBody = interactor.GetComponent<Rigidbody2D>();
        Interaction = InteractionTypes.Hand;
    }

    //chamado pelo PlayerInteractable ao soltar (botão de novo ou pulo)
    public void EndInteraction()
    {
        joint.enabled = false;
        Interaction = InteractionTypes.Alone;
    }
}