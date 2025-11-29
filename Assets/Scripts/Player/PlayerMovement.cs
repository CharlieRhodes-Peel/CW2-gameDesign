using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Relative to tilemap player should be able to:
    // - Jump up to a 4 block high platform
    // - Jump across a 6 block gap
    
    [Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpReleaseDamping;
    [SerializeField] private float flipTime;
    [SerializeField] private float maxVelocity;

    [SerializeField] private Transform coyoteJumpPos;
    [SerializeField] private float coyoteJumpRadius;
    
    [Header("References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference movingInput;
    [SerializeField] private InputActionReference jumpInput;
    
    [Header("Visuals")]
    [SerializeField] private Animator animator;
    
    //Privates
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private bool facingRight = true;
    
    //Events
    [HideInInspector] public static event Action<Vector2> ChangedLookDir;
    
    //Get components on player
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    //Subscribes to events when player is enabled and vice versa
    private void OnEnable()
    {
        //Input events
        jumpInput.action.started += Jump;
        jumpInput.action.canceled += CancelJump;
    }

    private void OnDisable()
    {
        jumpInput.action.started -= Jump;
        jumpInput.action.canceled -= CancelJump;
    }
    
    //Get player input
    void Update()
    {
        moveDirection = movingInput.action.ReadValue<Vector2>();
    }

    //Move player
    private void FixedUpdate()
    {
        //Only moves the player along the horizontal axis
        rb.linearVelocityX = Mathf.Round(moveDirection.x) * moveSpeed;
        
        animator.SetFloat("Move", Mathf.Abs(moveDirection.x));
        animator.SetBool("isGrounded", IsGrounded());

        if (rb.linearVelocityY < 0)
        {
            animator.SetBool("isFalling", true);
        }
        else {animator.SetBool("isFalling", false);}

        if (rb.linearVelocityY < -maxVelocity)
        {
            rb.linearVelocityY = -maxVelocity;
        }
        
        //If player is facing wrong way, flip them
        if (facingRight && moveDirection.x < 0 || !facingRight && moveDirection.x > 0)
        {
            Flip();
        }
    }
    
    //Called when jump button pressed down
    private void Jump(InputAction.CallbackContext ctx)
    {
        if(!IsGrounded()) { return; } //Don't jump if alr in air
        
        //Jump
        rb.linearVelocityY = 0;
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        
        animator.SetTrigger("Jump");
    }

    //Called when jump button released
    private void CancelJump(InputAction.CallbackContext ctx)
    {
        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocityY *= (1/jumpReleaseDamping);
        }
        
        //animator.SetTrigger("Fall");
    }

    //Checks if the player is grounded when called
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer) 
               || Physics2D.OverlapCircle(coyoteJumpPos.position, coyoteJumpRadius, groundLayer);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        LeanTween.rotateY(gameObject, facingRight ? 0 : 180, flipTime).setEaseInOutSine();
        ChangedLookDir?.Invoke(moveDirection);
    }

    public bool GetIsGrounded()
    {
        return IsGrounded();
    }
}
