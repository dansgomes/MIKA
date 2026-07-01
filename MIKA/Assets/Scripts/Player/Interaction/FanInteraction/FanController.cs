using UnityEngine;

public class FanController : MonoBehaviour
{
    [Header("Fan Settings")]
    public Vector2 windDirection = new Vector2(-1f, 0f);
    public float windForce = 10f;

    [Header("Detection")]
    public LayerMask playerLayer;

    private BoxCollider2D boxCollider;
    private bool isActive = true;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return;
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        playerRb.AddForce(windDirection.normalized * windForce, ForceMode2D.Force);
    }

    public void Deactivate()
    {
        if (!isActive) return;

        isActive = false;

        if (boxCollider != null)
            boxCollider.enabled = false;
    }

    public bool IsActive => isActive;
}