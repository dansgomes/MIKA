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
    public bool HoldsPlayer => true;

    private bool isLocked = false;

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

    public void Interact(GameObject interactor)
    {
        if (isLocked) return;

        joint.enabled = true;
        joint.connectedBody = interactor.GetComponent<Rigidbody2D>();
        Interaction = InteractionTypes.Hand;
    }

    public void EndInteraction()
    {
        if (isLocked) return;

        joint.enabled = false;
        Interaction = InteractionTypes.Alone;
    }

    public void Lock()
    {
        isLocked = true;

        if (joint != null)
        {
            joint.enabled = false;
        }

        rb.bodyType = RigidbodyType2D.Static;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        enabled = false;
    }
}