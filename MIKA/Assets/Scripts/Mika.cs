using UnityEngine;
using UnityEngine.InputSystem;

public class Mika : MonoBehaviour
{
      private InputSystem_Actions playerinput;
   private InputAction move;
   private InputAction jump;
   private Rigidbody2D rb;
   [SerializeField]private float jforce = 50;
   private Transform ground_pivot;
   [SerializeField] private LayerMask ground_layer;
   private Animator animator;
   private SpriteRenderer sprite;

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
       //animator = GetComponent<Animator>();
       rb = GetComponent<Rigidbody2D>();
       sprite = GetComponent<SpriteRenderer>();
       ground_pivot = transform.GetChild(0);
   }
   
   void Update()
   {
       detectGround();

       if(jump.WasPressedThisFrame() == true
       && detectGround() == true)
       {
          jump_action();
       }
   }

   void FixedUpdate()
   {
       move_action();
   }

   private bool detectGround()
   {
       Collider2D col = Physics2D.OverlapCircle(
           ground_pivot.position, 0.1f, ground_layer);
       
           if(col == null)
           {
               animator.SetBool("is_jumping", true);

               return false;
           }
           else
           {            
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
}
