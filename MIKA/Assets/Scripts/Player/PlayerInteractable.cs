using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractable : MonoBehaviour
{
    public float distance = 1f;
    public LayerMask boxMask;

    private InputSystem_Actions playerinput;
    private InputAction interact;

    private PlayerMovement playerMovement;

    private GameObject box;

    void Awake()
    {
        playerinput = new InputSystem_Actions();

        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        interact = playerinput.player.interact;
        interact.Enable();
    }

    void OnDisable()
    {
        interact.Disable();
    }

    void Start()
    {
        Physics2D.queriesStartInColliders = false;
    }

    void Update()
    {
        Vector2 facingDirection = playerMovement.FacingDirection;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, distance, boxMask);

        if (box == null && hit.collider != null && hit.collider.gameObject.tag == "pushable" && interact.WasPressedThisFrame())
        {
            box = hit.collider.gameObject;

            box.GetComponent<FixedJoint2D>().enabled = true;
            box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();

            BoxInteract boxScript = box.GetComponent<BoxInteract>();
            if (boxScript != null)
            {
                boxScript.Interaction = InteractionTypes.Hand;
            }
        }
        else if (interact.WasPressedThisFrame() && box != null)
        {
            box.GetComponent<FixedJoint2D>().enabled = false;

            BoxInteract boxScript = box.GetComponent<BoxInteract>();
            if (boxScript != null)
            {
                boxScript.Interaction = InteractionTypes.Alone;
            }

            box = null;
        }
    }

    void OnDrawGizmos()
    {
        if (playerMovement == null) return;
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + playerMovement.FacingDirection * distance);
    }
}