using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDamage : MonoBehaviour
{
    [Header("References")]
    //referencias aos componentes do player
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public PlayerMovement playerMovement;

    [Header("Knockback")]
    //duração do periodo de knockback
    public float knockbackDuration = 0.2f;

    [Header("Flash")]
    //quantidade de vezes que o player vai piscar
    public int flashCount = 3;
    //intervali entre as piscadas
    public float flashInterval = 0.08f;

    [Header("Collision Damage")]
    //camada de verificação de colisão
    public LayerMask damageLayer;
    //força de empurrada do knockback
    public float knockbackForce = 10f;
    //força vertical da empurrada
    public float knockbackVertical = 5f;
    
    //bool de verificação de se está tomando dano
    private bool isTakingDamage;

    //metodo usado para pegar os componentes do player no inicio da cena
    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    //metodo de controle de dano, recebe uma direção de empurrar
    public void TakeDamage(Vector2 knockbackForce)
    {
        //retorna se não puder tomar dano
        if (isTakingDamage) return;

        //começa a coroutine que controla o knockback
        StartCoroutine(DamageRoutine(knockbackForce));
    }

    //coroutine, age por um tempo determinado
    private IEnumerator DamageRoutine(Vector2 knockbackForce)
    {
        //player está tomando dano
        isTakingDamage = true;

        //verifica se a variável é acessável, se sim, a torna true
        if (playerMovement != null)
            playerMovement.isKnockedBack = true;

        //zera a velocidade e aplica um impulso na direção oposta ao dano
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);

        //começa a coroutine de flash de dano
        StartCoroutine(FlashRoutine());
        
        //usa yield para manter o player travado pelo tempo do knockback
        yield return new WaitForSeconds(knockbackDuration);

        //reseta o knockback do player, devolvendo o controle pra ele
        if (playerMovement != null)
            playerMovement.isKnockedBack = false;

        //pode tomar dano de novo
        isTakingDamage = false;
    }

    //coroutine de flashes
    private IEnumerator FlashRoutine()
    {
        //procura pelo renderer
        if (spriteRenderer == null) yield break;

        //usa um loop pra repetir pelo numero de iterações
        for (int i = 0; i < flashCount; i++)
        {
            //desativa o renderer, espera alguns segundos, ativa de novo
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    //quando colide com algo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //verifica se a layer do objeto é a layer de dano
        if ((damageLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        //define a direção pela posição do objeto menos a posição do colisor
        Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;

        //define a direção y com valor máximo
        knockbackDir.y = Mathf.Max(knockbackDir.y, 0.3f);
        knockbackDir = knockbackDir.normalized;

        //chama a função de dano
        TakeDamage(knockbackDir * knockbackForce);
    }
}