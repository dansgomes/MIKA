using UnityEngine;

public class FallingObject : MonoBehaviour
{
    [Header("Destruction")]
    public LayerMask destroyOnLayer;

    [Header("Player Contact")]
    public bool destroyOnContactWithPlayer = false;
    public LayerMask playerLayer;
    public bool damagePlayerOnContact = false;
    public float knockbackForce = 8f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        if ((destroyOnLayer.value & (1 << collisionLayer)) != 0)
        {
            Destroy(gameObject);
            return;
        }

        if (destroyOnContactWithPlayer && (playerLayer.value & (1 << collisionLayer)) != 0)
        {
            if (damagePlayerOnContact)
            {
                PlayerDamage playerDamage = collision.gameObject.GetComponent<PlayerDamage>();
                if (playerDamage != null)
                {
                    Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                    knockbackDir.y = Mathf.Max(knockbackDir.y, 0.3f);
                    knockbackDir = knockbackDir.normalized;

                    playerDamage.TakeDamage(knockbackDir * knockbackForce);
                }
            }

            Destroy(gameObject);
        }
    }
}