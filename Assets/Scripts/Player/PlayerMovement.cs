using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpReleaseDamping;
    
    [Header("References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference movingInput;
    [SerializeField] private InputActionReference jumpInput;
    
    //Privates
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    
    //Get components on player
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        rb.linearVelocityX = moveDirection.x * moveSpeed;
    }
    
    //Called when jump button pressed down
    private void Jump(InputAction.CallbackContext ctx)
    {
        if(!IsGrounded()) { return; } //Don't jump if alr in air
        
        //Jump
        rb.linearVelocityY = 0;
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    //Called when jump button released
    private void CancelJump(InputAction.CallbackContext ctx)
    {
        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocityY *= (1/jumpReleaseDamping);
        }
    }

    //Checks if the player is grounded when called
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    
    //Subscribes to input events when player is enabled and vice versa
    private void OnEnable()
    {
        jumpInput.action.started += Jump;
        jumpInput.action.canceled += CancelJump;
    }

    private void OnDisable()
    {
        jumpInput.action.started -= Jump;
        jumpInput.action.canceled -= CancelJump;
    }
}
