using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingPlatform : MonoBehaviour
{
    public event Action OnPlatformLanded;

    [Header("Collision")]
    public LayerMask playerLayer;

    [Header("Landing Detection")]
    public float landingVelocityThreshold = 0.05f;
    public float landingCheckDuration = 0.1f;

    private Rigidbody2D rb;
    private Collider2D myCollider;
    private bool isFalling;
    private bool hasLanded;
    private float stillTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    public void Fall()
    {
        if (isFalling || hasLanded) return;

        rb.bodyType = RigidbodyType2D.Dynamic;
        isFalling = true;
    }

    void FixedUpdate()
    {
        if (!isFalling || hasLanded) return;

        if (Mathf.Abs(rb.linearVelocity.y) <= landingVelocityThreshold)
        {
            stillTimer += Time.fixedDeltaTime;

            if (stillTimer >= landingCheckDuration)
            {
                Land();
            }
        }
        else
        {
            stillTimer = 0f;
        }
    }

    private void Land()
    {
        hasLanded = true;
        isFalling = false;

        myCollider.excludeLayers |= playerLayer;

        OnPlatformLanded?.Invoke();
    }
}