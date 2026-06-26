using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDamage : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public PlayerMovement playerMovement;

    [Header("Knockback")]
    public float knockbackDuration = 0.2f;

    [Header("Collision Damage")]
    public LayerMask damageLayer;
    public float knockbackForce = 10f;
    public float knockbackVertical = 5f;
    
    private bool isTakingDamage;
    public bool IsTakingDamage => isTakingDamage;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(Vector2 knockbackForce)
    {
        if (isTakingDamage) return;

        StartCoroutine(DamageRoutine(knockbackForce));
    }

    private IEnumerator DamageRoutine(Vector2 knockbackForce)
    {
        isTakingDamage = true;

        if (playerMovement != null)
            playerMovement.isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        if (playerMovement != null)
            playerMovement.isKnockedBack = false;

        isTakingDamage = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((damageLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;

        knockbackDir.y = Mathf.Max(knockbackDir.y, 0.3f);
        knockbackDir = knockbackDir.normalized;

        TakeDamage(knockbackDir * knockbackForce);
    }
}