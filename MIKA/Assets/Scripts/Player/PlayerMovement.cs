using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //evento disparado sempre que pula
    public event Action OnJumped;

    [Header("Movement")]
    //referencia ao rigidbody do player
    public Rigidbody2D rb;
    //velocidade base de movimento
    public float moveSpeed = 5f;
    public float crouchSpeedDivider = 2f;
    public float runSpeedMultiplier = 1.6f;
    float horizontalMovement;
    float verticalInput;

    [Header("Run")]
    //multiplicador de velocidade ao correr
    
    //bool de verificação se está correndo
    bool isRunning;

    [Header("External Control")]
    //bool controlada externamente para travar o movimento durante knockback
    public bool isKnockedBack;

    [Header("Crouch")]
    //referencia ao sprite renderer para controle de opacidade
    public SpriteRenderer spriteRenderer;
    //opacidade do sprite ao agachar
    public float crouchOpacity = 0.5f;
    //bool de verificação se está agachado
    bool isCrouching;

    [Header("Acceleration")]
    //tempo de suavização da aceleração horizontal
    public float accelerationTime = 0f;
    //variáveis auxiliares do SmoothDamp
    private float velocityXSmoothing;
    private float currentVelocityX;

    [Header("Jump")]
    //força do pulo
    public float jumpPower = 10f;
    //bool de verificação da direção que o player está olhando
    public bool isFacingRight = true;
    //bool de verificação se pode pular
    bool canJumpNow;

    //direção horizontal que o player está olhando
    public Vector2 FacingDirection => new Vector2(isFacingRight ? 1f : -1f, 0f);

    [Header("Ground Check")]
    //posição do pivot de chão
    public Transform groundCheckPos;
    //tamanho da caixa de verificação de chão
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    //camada de verificação de chão
    public LayerMask groundLayer;
    //bool de verificação se está no chão
    public bool inGround;
    //bool de verificação se pode pular
    public bool canJump;

    [Header("Gravity")]
    //gravidade base
    public float baseGravity = 2;
    //velocidade máxima de queda
    public float maxFallSpeed = 18f;
    //multiplicador de gravidade ao cair
    public float fallSpeedMultiplier = 2f;

    [Header("Check Walls")]
    //posição do pivot de parede
    public Transform wallCheckPos;
    //tamanho da caixa de verificação de parede
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    //camada de verificação de parede
    public LayerMask wallLayer;

    [Header("Ledge Grab")]
    //camada de verificação de quina
    public LayerMask ledgeLayer;
    //pivot de detecção de parede da quina
    public Transform ledgeWallCheckPos;
    //pivot de detecção do topo da quina
    public Transform ledgeTopCheckPos;
    //distância dos raycasts de detecção
    public float ledgeCheckDistance = 0.5f;
    //deslocamento aplicado ao subir a quina
    public Vector2 ledgeClimbOffset = new Vector2(0.3f, 1f);
    //bool de verificação se está pendurado na quina
    bool isLedgeGrabbed;
    //posição registrada no momento do grab
    Vector2 ledgeGrabPosition;

    private void Update()
    {
        //atualiza verificação de chão todo frame
        GroundCheck();

        //trava toda lógica durante o knockback
        if (isKnockedBack)
        {
            return;
        }

        //cancela o agachamento se mover ou correr
        if (isCrouching && (!inGround || isRunning))
        {
            SetCrouching(false);
        }

        //processa input enquanto pendurado na quina
        if (isLedgeGrabbed)
        {
            HandleLedgeGrabbedInput();
            return;
        }

        //verifica e executa o grab de quina se estiver caindo e detectar uma
        if (!inGround && rb.linearVelocity.y <= 0f && CheckForLedge())
        {
            GrabLedge();
            return;
        }

        //aplica a gravidade customizada
        Gravity();

        //aplica o movimento horizontal com suavização
        float targetSpeed = moveSpeed;
        if (isRunning)
        {
            targetSpeed *= runSpeedMultiplier;
        }
        else if (isCrouching)
        {
            targetSpeed /= crouchSpeedDivider;
        }
        currentVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, horizontalMovement * targetSpeed, ref velocityXSmoothing, accelerationTime);
        rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

        Flip();
    }

    //retorna true se há uma parede no pivot de verificação
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    //retorna true se os dois raycasts detectam uma quina válida
    private bool CheckForLedge()
    {
        //reforça a regra: nunca agarra quina estando no chão, independente de quem chamar este método
        if (inGround) return false;

        Vector2 facingDirection = new Vector2(isFacingRight ? 1f : -1f, 0f);

        //raycast na altura do peito: deve bater na parede
        RaycastHit2D wallHit = Physics2D.Raycast(ledgeWallCheckPos.position, facingDirection, ledgeCheckDistance, ledgeLayer);
        //raycast acima da cabeça: não deve bater em nada
        RaycastHit2D topHit = Physics2D.Raycast(ledgeTopCheckPos.position, facingDirection, ledgeCheckDistance, ledgeLayer);

        return wallHit.collider != null && topHit.collider == null;
    }

    //trava o player na quina e zera a gravidade
    private void GrabLedge()
    {
        isLedgeGrabbed = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        //registra a posição do grab para calcular o destino da subida
        ledgeGrabPosition = transform.position;
    }

    //processa o input enquanto pendurado
    private void HandleLedgeGrabbedInput()
    {
        //mantém o player parado enquanto pendurado
        rb.linearVelocity = Vector2.zero;

        //apertou para cima, sobe a quina instantaneamente
        if (verticalInput > 0.1f)
        {
            ClimbLedge();
        }
        //apertou para baixo, solta a quina
        else if (verticalInput < -0.1f)
        {
            DropFromLedge();
        }
    }

    //solta a quina e restaura a gravidade
    private void DropFromLedge()
    {
        isLedgeGrabbed = false;
        rb.gravityScale = baseGravity;
    }

    //teleporta o player do ponto de grab até o topo da quina, sem animação de subida
    private void ClimbLedge()
    {
        isLedgeGrabbed = false;

        //calcula o ponto de destino baseado na direção que o player olha
        float facingSign = isFacingRight ? 1f : -1f;
        Vector2 targetPosition = ledgeGrabPosition + new Vector2(ledgeClimbOffset.x * facingSign, ledgeClimbOffset.y);

        //aplica a posição final diretamente e restaura a física
        transform.position = targetPosition;
        rb.gravityScale = baseGravity;
    }

    //aumenta a gravidade ao cair e limita a velocidade máxima de queda
    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    //callback de input, lê o movimento horizontal e vertical
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontalMovement = input.x;
        verticalInput = input.y;
    }

    //callback de input, ativa e desativa o estado de corrida
    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
            isRunning = true;
        else if (context.canceled)
            isRunning = false;
    }

    //callback de input, ativa e desativa o agachamento
    public void Crouch(InputAction.CallbackContext context)
    {
        //não pode agachar correndo ou no ar
        if (isRunning || !inGround) return;

        if (context.performed)
            SetCrouching(true);
        else if (context.canceled)
            SetCrouching(false);
    }

    //aplica ou remove o estado de agachamento e ajusta a opacidade do sprite
    private void SetCrouching(bool value)
    {
        isCrouching = value;

        if (spriteRenderer == null) return;

        //deixa o sprite semi-transparente ao agachar
        Color color = spriteRenderer.color;
        color.a = isCrouching ? crouchOpacity : 1f;
        spriteRenderer.color = color;
    }

    //callback de input, processa o pulo simples e o pulo de quina
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //pulo de quina
            if (isLedgeGrabbed)
            {
                isLedgeGrabbed = false;
                rb.gravityScale = baseGravity;
                rb.linearVelocity = new Vector2(-(isFacingRight ? 1f : -1f) * jumpPower, jumpPower);
                OnJumped?.Invoke();
                return;
            }

            //pulo normal
            if (canJumpNow && (inGround || canJump))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                canJumpNow = false;
                OnJumped?.Invoke();
            }
        }
        else if (context.canceled)
        {
            //corta o pulo ao soltar o botão, permitindo pulos mais curtos
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    //O Jogo (só pra saber se tu tá esperto msm kkkkk)

    //verifica se está no chão ou na parede e gerencia a liberação do pulo
    private void GroundCheck()
    {
        inGround = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        canJump = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, wallLayer);

        //só restaura o pulo disponível se estiver no chão/parede E não estiver subindo
        //(evita re-liberar o pulo no mesmo frame em que ele decolou, antes de saída da caixa de detecção)
        if ((inGround || canJump) && rb.linearVelocity.y <= 0.01f)
        {
            canJumpNow = true;
        }
    }

    //inverte o sprite e a flag de direção quando o movimento muda de lado
    public void Flip()
    {
        if ((isFacingRight && horizontalMovement < 0) || (!isFacingRight && horizontalMovement > 0))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //caixa de verificação de chão
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        //caixa de verificação de parede
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);

        //raio de detecção de quina
        if (ledgeWallCheckPos != null)
        {
            Gizmos.color = Color.green;
            Vector2 dir = new Vector2(isFacingRight ? 1f : -1f, 0f) * ledgeCheckDistance;
            Gizmos.DrawLine(ledgeWallCheckPos.position, (Vector2)ledgeWallCheckPos.position + dir);
        }

        //raio de detecção de quina
        if (ledgeTopCheckPos != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 dir = new Vector2(isFacingRight ? 1f : -1f, 0f) * ledgeCheckDistance;
            Gizmos.DrawLine(ledgeTopCheckPos.position, (Vector2)ledgeTopCheckPos.position + dir);
        }
    }
}