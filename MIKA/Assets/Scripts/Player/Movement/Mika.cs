using UnityEngine;
using UnityEngine.InputSystem;

public class Mika : MonoBehaviour
{
    [Header("Configuracoes de Borda")]
    public Transform ledgeCheck;
    public Transform ledgeTopCheck;
    public LayerMask ledgeLayer;
    public float checkDistance = 5f;
    public Vector2 climbOffset = new Vector2(0.5f, 1.5f); // Quanto ele vai para frente e para cima ao subir
    private bool isHanging = false;
    private Vector2 targetHangingPosition;

   private InputSystem_Actions playerinput;
   private InputAction move;
   private InputAction jump;
   private Rigidbody2D rb;
   [SerializeField]private float jforce = 50;
   [SerializeField] private Transform ground_pivot;
   [SerializeField] private LayerMask ground_layer;
   private Animator animator;
   private SpriteRenderer sprite;
   public Vector2 currentDir;

   // Movement Variables
   private float speed = 15;
   private Vector2 m_Velocity = Vector2.zero;
   [Range(0.05f, 0.3f)]
   private float m_MovementSmoothing = 0.1f;

   private bool isOnGround = false;

   void Awake()
   {
       playerinput = new InputSystem_Actions();
   }

   void OnEnable()
   {
       move = playerinput.player.move;
       move.Enable();
       jump = playerinput.player.jump;
       jump.Enable();
   }

   void OnDisable()
   {
       move.Disable();
       jump.Disable();
   }
   
   void Start()
   {
       animator = GetComponent<Animator>();
       rb = GetComponent<Rigidbody2D>();
       sprite = GetComponent<SpriteRenderer>();
       ground_pivot = transform.GetChild(0);
   }

    void Update()
    {
        // Armazena o resultado do chao uma unica vez por frame
        isOnGround = detectGround();

        // Verifica se apertou espaço E se esta firmemente no chao
        if (jump.triggered && isOnGround)
        {
            jump_action();
        }

        if (!isHanging)
        {
            CheckForLedge();
        }
        else
        {
            //Força o personagem a ficar imóvel horizontalmente na borda
            rb.linearVelocity = Vector2.zero;
            float inputVertical = 0;

            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed ||
                    UnityEngine.InputSystem.Keyboard.current.upArrowKey.isPressed)
                {
                    inputVertical = 1f; // Apertou para subir
                }
                else if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed ||
                         UnityEngine.InputSystem.Keyboard.current.downArrowKey.isPressed)
                {
                    inputVertical = -1f; // Apertou para soltar
                }
            }
            // ------------------------------------------

            // Processa a subida
            if (inputVertical > 0)
            {
                ClimbLedge();
            }
            // Processa a soltura
            else if (inputVertical < 0)
            {
                DropLedge();
            }
        }
    }

    void FixedUpdate()
   {
        move_action();

        if (isHanging)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private bool detectGround()
    {
        // Desenha o circulo invis�vel para checar o ch�o
        Collider2D col = Physics2D.OverlapCircle(ground_pivot.position, 0.3f, ground_layer);

        if (col == null)
        {
            // Se NaO encontrou o ch�o: ativa anima��o de pulo e retorna FALSE (n�o pode pular)
            animator.SetBool("is_jumping", true);
            return false;
        }
        else
        {
            // Se ENCONTROU o ch�o: desativa anima��o de pulo e retorna TRUE (pode pular)
            animator.SetBool("is_jumping", false);
            return true;
        }
    }

    private void jump_action()
   {
       rb.AddForce(transform.up * jforce, ForceMode2D.Impulse);
   }

   private void move_action()
   {
       Vector2 moveValue = move.ReadValue<Vector2>();
       
       if (moveValue != Vector2.zero)
        {
            currentDir = new Vector2(moveValue.x, 0);
        }

        Debug.Log(currentDir);

       if (moveValue.x == 0)
       {
           animator.SetBool("is_running", false);
       }
       else
       {
           if(moveValue.x > 0) // invert sprite direction
           {
               sprite.flipX = false;
           }
           else if(moveValue.x < 0)
           {
               sprite.flipX = true;
           }
           animator.SetBool("is_running", true);
       }

       Vector2 targetVelocity = 
            new Vector2(moveValue.x * speed * 10 * 
            Time.fixedDeltaTime, rb.linearVelocity.y);

       rb.linearVelocity =
            Vector2.SmoothDamp(rb.linearVelocity,
            targetVelocity, ref m_Velocity,
            m_MovementSmoothing);

   }

    void CheckForLedge()
    {
        // 1. Dispara o raio de baixo (no peito/mão)
        RaycastHit2D hitBaixo = Physics2D.Raycast(ledgeCheck.position, transform.right * currentDir, checkDistance, ledgeLayer);

        // 2. Dispara o raio de cima (acima da cabeça)
        RaycastHit2D hitCima = Physics2D.Raycast(ledgeTopCheck.position, transform.right * currentDir, checkDistance, ledgeLayer);

        // LÓGICA DA QUINA: O de baixo bate na parede E o de cima NÃO bate em nada
        if (hitBaixo.collider != null && hitCima.collider == null && rb.linearVelocity.y <= 0)
        {
            Debug.Log("grabed");
            GrabLedge(hitBaixo.point);
        }
    }

    void GrabLedge(Vector2 hitPoint)
    {
        isHanging = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        // Trava o personagem na posi��o do impacto (ajuste conforme necess�rio)
        transform.position = new Vector2(hitPoint.x - (0.2f * transform.localScale.x), hitPoint.y);
    }

    void ClimbLedge()
    {
        // Calcula a posi��o final (topo da plataforma) baseado para onde o personagem olha
        Vector2 finalPosition = new Vector2(transform.position.x + (climbOffset.x * currentDir.x), transform.position.y + climbOffset.y);

        // Teleporta o jogador para cima da plataforma
        transform.position = finalPosition;

        // Restaura a f�sica
        rb.gravityScale = 1;
        isHanging = false;
    }

    void DropLedge()
    {
        rb.gravityScale = 1;
        isHanging = false;
    }

    private void OnDrawGizmos()
    {
        // Desenha o raio no editor da Unity para ajudar a calibrar
        if (ledgeCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ledgeCheck.position, currentDir * checkDistance);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(ledgeTopCheck.position, currentDir * checkDistance);
        }
    }
}
