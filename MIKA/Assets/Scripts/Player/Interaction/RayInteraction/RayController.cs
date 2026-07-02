using UnityEngine;

public class RayController : MonoBehaviour
{

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