using UnityEngine;

public enum InteractionTypes
{
    Alone,
    Hand
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(RelativeJoint2D))]
public class BoxInteract : MonoBehaviour, IInteractable
{
    private Rigidbody2D rb;
    private RelativeJoint2D joint;
    public bool HoldsPlayer => true;

    [Header("Relative Joint Settings")]
    public float maxForce = 1000f;
    public float maxTorque = 0f;

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
        joint = GetComponent<RelativeJoint2D>();
        joint.enabled = false;
        joint.maxForce = maxForce;
        joint.maxTorque = maxTorque;
        Interactions();
    }

    private void Interactions()
    {
        switch (interaction)
        {
            case InteractionTypes.Alone:
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.mass = 15;
                rb.constraints = RigidbodyConstraints2D.None;
                break;

            case InteractionTypes.Hand:
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.mass = 1;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                break;
        }
    }

    public void Interact(GameObject interactor)
    {
        if (isLocked) return;

        Rigidbody2D playerRb = interactor.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        Vector2 currentOffset = rb.position - playerRb.position;

        joint.connectedBody = playerRb;
        joint.linearOffset = currentOffset;
        joint.angularOffset = 0f;
        joint.enabled = true;

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