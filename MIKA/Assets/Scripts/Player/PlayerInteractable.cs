using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractable : MonoBehaviour
{
    public float distance = 1f;
    public LayerMask boxMask;

    private InputSystem_Actions playerinput;
    private InputAction interact;

    private GameObject box;
    void Awake()
    {
        playerinput = new InputSystem_Actions();
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Physics2D.queriesStartInColliders = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, distance, boxMask);

        if (box == null && hit.collider != null && hit.collider.gameObject.tag == "pushable" && interact.WasPressedThisFrame())
        {
            box = hit.collider.gameObject;

            box.GetComponent<FixedJoint2D>().enabled = true;
            box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
        } 
        else if (interact.WasPressedThisFrame() && box != null)
        {
            box.GetComponent<FixedJoint2D>().enabled = false;
            box = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * transform.localScale.x * distance);
    }
}
