using UnityEngine;

public enum InteractionTypes
{
    Alone,
    Hand
}


public class BoxInteract : MonoBehaviour
{
    private Rigidbody2D rb;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
}