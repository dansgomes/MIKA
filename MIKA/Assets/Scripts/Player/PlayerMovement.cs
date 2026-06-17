using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    //referencia ao rigidbody do player
    public Rigidbody2D rb;
    //velocidade base de movimento
    public float moveSpeed = 5f;
    //entrada horizontal e vertical do jogador
    float horizontalMovement;
    float verticalInput;
    //direção atual do movimento (-1, 0 ou 1)
    private int currentDirection = 0;

    [Header("Run")]
    //multiplicador de velocidade ao correr
    public float runSpeedMultiplier = 1.6f;
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
    bool isFacingRight = true;
    //numero máximo de pulos permitidos
    public int maxJumps = 2;
    //pulos restantes disponíveis
    int jumpsRemaining;

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

    [Header("Wall Movement")]
    //velocidade de deslizamento na parede
    public float wallSlideSpeed = 2;
    //bool de verificação se está deslizando na parede
    bool isWallSliding;

    [Header("Wall Jump")]
    //bool de verificação se está em wall jump
    bool isWallJumping;
    //direção do wall jump
    float wallJumpDirection;
    //janela de tempo para executar o wall jump
    float wallJumpTime = 0.3f;
    //timer da janela de wall jump
    float wallJumpTimer;
    //força do wall jump (x = horizontal, y = vertical)
    public Vector2 wallJumpPower = new Vector2(5f, 10f);
    //bool de verificação se pode executar wall jump
    bool canWallJump = false;

    [Header("Dash")]
    //velocidade do dash
    public float dashSpeed = 12f;
    //duração do dash
    public float dashDuration = 0.2f;
    //cooldown entre dashes
    public float dashCooldown = 0.5f;
    //tempo de suavização ao sair do dash
    public float dashEndSmoothTime = 0.15f;
    //duração da suavização ao sair do dash
    public float dashEndSmoothDuration = 0.2f;
    //bool de verificação se está em dash
    bool isDashing;
    //timer de duração do dash
    float dashTimer;
    //timestamp do ultimo dash executado
    float lastDashTime;
    //ultima direção registrada antes do dash
    int lastDirection;
    //timer da suavização pós-dash
    float dashEndSmoothTimer;

    [Header("Wall Climb")]
    //velocidade de escalada na parede
    public float wallClimbSpeed = 2f;

    [Header("Ledge Grab")]
    //camada de verificação de quina
    public LayerMask ledgeLayer;
    //pivot de detecção de parede da quina
    public Transform ledgeWallCheckPos;
    //pivot de detecção do topo da quina
    public Transform ledgeTopCheckPos;
    //distância dos raycasts de detecção
    public float ledgeCheckDistance = 0.5f;
    //deslocamento aplicado ao subir a quina (x = frente, y = cima)
    public Vector2 ledgeClimbOffset = new Vector2(0.3f, 1f);
    //duração da animação de subida da quina
    public float ledgeClimbDuration = 0.25f;
    //bool de verificação se está pendurado na quina
    bool isLedgeGrabbed;
    //bool de verificação se está no meio da subida
    bool isLedgeClimbing;
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

        //trava a lógica enquanto a animação de subida ocorre
        if (isLedgeClimbing)
        {
            return;
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

        //processa o deslize e pulo de parede
        ProcessWallSlide();
        ProcessWallJump();

        //sobe na parede se estiver deslizando e apertar para cima
        if (isWallSliding && verticalInput > 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallClimbSpeed);
        }

        //processa o dash ativo, contando o timer e aplicando a velocidade
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                //finaliza o dash e inicia a suavização de saída
                isDashing = false;
                dashEndSmoothTimer = dashEndSmoothDuration;
            }
            else
            {
                //mantém a velocidade do dash e interrompe o resto da lógica
                rb.linearVelocity = new Vector2(lastDirection * dashSpeed, 0f);
                GroundCheck();
                return;
            }
        }

        //aplica o movimento horizontal com suavização
        if (!isWallJumping && !isDashing)
        {
            float smoothTime = accelerationTime;

            //usa suavização maior logo após o dash acabar
            if (dashEndSmoothTimer > 0f)
            {
                dashEndSmoothTimer -= Time.deltaTime;
                smoothTime = dashEndSmoothTime;
            }

            //calcula a velocidade alvo e aplica via SmoothDamp
            float targetSpeed = moveSpeed * (isRunning ? runSpeedMultiplier : 1f);
            currentVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, horizontalMovement * targetSpeed, ref velocityXSmoothing, smoothTime);
            rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

            Flip();
        }
    }

    //retorna true se há uma parede no pivot de verificação
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    //retorna true se os dois raycasts detectam uma quina válida
    private bool CheckForLedge()
    {
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

        //apertou para cima: inicia a subida
        if (verticalInput > 0.1f)
        {
            StartCoroutine(ClimbLedgeRoutine());
        }
        //apertou para baixo: solta a quina
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

    //coroutine que move o player suavemente do grab até o topo da plataforma
    private System.Collections.IEnumerator ClimbLedgeRoutine()
    {
        isLedgeGrabbed = false;
        isLedgeClimbing = true;

        //calcula o ponto de destino baseado na direção que o player olha
        float facingSign = isFacingRight ? 1f : -1f;
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = ledgeGrabPosition + new Vector2(ledgeClimbOffset.x * facingSign, ledgeClimbOffset.y);

        //interpola a posição do player ao longo do tempo configurado
        float elapsed = 0f;
        while (elapsed < ledgeClimbDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / ledgeClimbDuration);
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        //garante que chegou exatamente no destino e restaura a física
        transform.position = targetPosition;
        rb.gravityScale = baseGravity;
        isLedgeClimbing = false;
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

    //controla o deslizamento na parede enquanto o player se move contra ela no ar
    private void ProcessWallSlide()
    {
        if (!inGround && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            //limita a velocidade de queda ao valor do slide
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    //gerencia a janela de tempo disponível para executar o wall jump
    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            //enquanto na parede, mantém a janela aberta e registra a direção
            canWallJump = true;
            wallJumpDirection = -Mathf.Sign(transform.localScale.x);
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else
        {
            //conta o timer e fecha a janela quando expirar
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0f)
                canWallJump = false;
        }
    }

    //encerra o estado de wall jump, devolvendo controle ao movimento normal
    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    //callback de input: lê o movimento horizontal e vertical
    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontalMovement = input.x;
        verticalInput = input.y;

        //registra a direção atual para uso no dash
        if (horizontalMovement != 0)
            currentDirection = (int)Mathf.Sign(horizontalMovement);
    }

    //callback de input: ativa e desativa o estado de corrida
    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
            isRunning = true;
        else if (context.canceled)
            isRunning = false;
    }

    //callback de input: ativa e desativa o agachamento
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

    //callback de input: processa pulo, wall jump e pulo de quina
    public void Jump(InputAction.CallbackContext context)
    {
        //bloqueia o pulo durante a animação de subida de quina
        if (isLedgeClimbing) return;

        if (context.performed)
        {
            //pulo de quina: empurra o player na direção oposta à parede
            if (isLedgeGrabbed)
            {
                isLedgeGrabbed = false;
                rb.gravityScale = baseGravity;
                rb.linearVelocity = new Vector2(-(isFacingRight ? 1f : -1f) * wallJumpPower.x, wallJumpPower.y);
                return;
            }

            //wall jump: empurra o player para longe da parede
            if (canWallJump)
            {
                isWallJumping = true;
                canWallJump = false;

                rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
                wallJumpTimer = 0f;

                //inverte o sprite se o wall jump mudar a direção
                if ((wallJumpDirection > 0 && transform.localScale.x < 0) ||
                    (wallJumpDirection < 0 && transform.localScale.x > 0))
                {
                    isFacingRight = !isFacingRight;
                    Vector3 ls = transform.localScale;
                    ls.x *= -1f;
                    transform.localScale = ls;
                }

                //encerra o wall jump após o tempo configurado
                Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
            }
            //pulo normal: consome um pulo disponível
            else if (jumpsRemaining > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
            }
        }
        else if (context.canceled)
        {
            //corta o pulo ao soltar o botão, permitindo pulos mais curtos
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    //O Jogo (só pra saber se tu tá esperto msm kkkkk)

    //callback de input: executa o dash na direção atual se o cooldown permitir
    public void Dash(InputAction.CallbackContext context)
    {
        //bloqueia o dash durante grab ou subida de quina
        if (isLedgeGrabbed || isLedgeClimbing) return;

        if (context.performed && !isDashing && Time.time - lastDashTime > dashCooldown)
        {
            if (currentDirection != 0)
            {
                //inicia o dash registrando direção, timer e timestamp
                lastDirection = currentDirection;
                isDashing = true;
                dashTimer = dashDuration;
                lastDashTime = Time.time;
                rb.linearVelocity = new Vector2(lastDirection * dashSpeed, 0f);
            }
        }
    }

    //verifica se está no chão ou na parede e gerencia os pulos disponíveis
    private void GroundCheck()
    {
        inGround = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        canJump = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, wallLayer);

        //restaura os pulos ao tocar o chão ou a parede
        if (inGround || canJump)
        {
            jumpsRemaining = maxJumps;
        }

        //cancela o wall jump ao pousar no chão
        if (inGround && isWallJumping)
        {
            CancelInvoke(nameof(CancelWallJump));
            isWallJumping = false;
        }
    }

    //inverte o sprite e a flag de direção quando o movimento muda de lado
    private void Flip()
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

        //raio de detecção de quina (inferior)
        if (ledgeWallCheckPos != null)
        {
            Gizmos.color = Color.green;
            Vector2 dir = new Vector2(isFacingRight ? 1f : -1f, 0f) * ledgeCheckDistance;
            Gizmos.DrawLine(ledgeWallCheckPos.position, (Vector2)ledgeWallCheckPos.position + dir);
        }

        //raio de detecção de quina (superior)
        if (ledgeTopCheckPos != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 dir = new Vector2(isFacingRight ? 1f : -1f, 0f) * ledgeCheckDistance;
            Gizmos.DrawLine(ledgeTopCheckPos.position, (Vector2)ledgeTopCheckPos.position + dir);
        }
    }
}