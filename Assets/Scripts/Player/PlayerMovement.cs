using System;
using System.Collections;
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

    [SerializeField] private float invulMovementPauseTime;

    [Header("Movement Power up Stats")] 
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private float wallClimbGravity;
    [SerializeField] private float wallJumpTime;
    [SerializeField] private float wallJumpForceX;
    [SerializeField] private float wallJumpForceY;
    
    [Header("References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference movingInput;
    [SerializeField] private InputActionReference jumpInput;
    [SerializeField] private InputActionReference dashInput;

    [Header("Visuals")] 
    [SerializeField] private Transform groundPartcilesPos;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject jumpParticles;
    [SerializeField] private GameObject landParticles;
    [SerializeField] private float landParticleSpeedExponent;
    [SerializeField] private float landParticleSizeMultiplier;
    [SerializeField] private GameObject dashParticles;
    
    [Header("Sounds")]
    [SerializeField] private AudioClip[] walkSounds;
    [SerializeField] private AudioClip[] jumpSounds;
    [SerializeField] private AudioClip slideSound;
    [SerializeField] private AudioClip[] dashSounds;
    [SerializeField] private AudioClip[] landSounds;
    
    //Privates
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private bool facingRight = true;
    [HideInInspector] public bool movementDisabled = false;
    private bool falling = false;
    
    //Movement PowerUps Flags
    [Header("Toggles")]
    public bool doubleJumpUnlocked = false;
    private bool doubleJumpPerformed = false;
    
    public bool dashUnlocked = false;
    private bool dashPerformed = false;
    [HideInInspector] public bool isDashing = false;
    private bool dashOnCooldown = false;

    public bool wallClimbingUnlocked = false;
    [HideInInspector] public bool isWallClimbing = false;
    private bool isWallJumping = false;
    private float baseGravity;
    private bool hitWallYet = false;

    private float fallVelocityReached;

    private bool isPlayingWalkSounds = false;

    public enum AbilityUnlocks
    {
        None,
        DoubleJump,
        Dash,
        WallClimbing
    }
        
    //Events
    [HideInInspector] public static event Action<Vector2> ChangedLookDir;
    
    //Get components on player
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        baseGravity = rb.gravityScale;
        
        PlayerHealth.OnPlayerHit += (float idc) => StartCoroutine(DisableMovementFor(invulMovementPauseTime));
    }
    
    //Subscribes to events when player is enabled and vice versa
    private void OnEnable()
    {
        //Input events
        jumpInput.action.started += Jump;
        jumpInput.action.canceled += CancelJump;
        dashInput.action.started += Dash;
    }

    private void OnDisable()
    {
        jumpInput.action.started -= Jump;
        jumpInput.action.canceled -= CancelJump;
        dashInput.action.started -= Dash;
    }
    
    //Get player input
    void Update()
    {
        moveDirection = movingInput.action.ReadValue<Vector2>();
    }

    //Move player
    private void FixedUpdate()
    {
        if (isDashing) { rb.linearVelocityY = 0;}
        
        if (movementDisabled) { return; }
        //Only moves the player along the horizontal axis
        rb.linearVelocityX = Mathf.Round(moveDirection.x) * moveSpeed;

        //Sounds
        if (isWalking() && !isPlayingWalkSounds)
        {
            StartCoroutine(PlayWalkSounds());
            isPlayingWalkSounds = true;
        }
        
        bool isGrounded = IsGrounded();   
        
        //Animator
        animator.SetFloat("Move", Mathf.Abs(rb.linearVelocityX));
        animator.SetBool("isGrounded", isGrounded);
        
        //Wall Cling Stuff
        if (IsWallClimbing() && !isWallJumping && wallClimbingUnlocked)
        {
            isWallClimbing = true;
            rb.gravityScale = wallClimbGravity;
            if (!hitWallYet)
            {
                ApplyVerticalDamping();
                hitWallYet = true;
            }
            
            animator.SetBool("IsWallClimbing", true);
            if (!SoundManager.Instance.IsPlayingSound(slideSound))
            {
                SoundManager.Instance.PlaySoundEffect(slideSound, transform, 0.5f);
            }
        }
        else
        {
            isWallClimbing = false;
            rb.gravityScale = baseGravity;
            hitWallYet = false;
            animator.SetBool("IsWallClimbing", false);
            SoundManager.Instance.StopSoundEffect(slideSound);
        }
        

        //Max velocity
        if (rb.linearVelocityY < -maxVelocity)
        {
            rb.linearVelocityY = -maxVelocity;
        }
        
        //If player is facing wrong way, flip them
        if (facingRight && moveDirection.x < 0 || !facingRight && moveDirection.x > 0)
        {
            Flip();
        }
        
        //Animator
        if (rb.linearVelocityY < 0)
        {
            falling = true;
            animator.SetBool("isFalling", true);
            fallVelocityReached = rb.linearVelocityY;
        }
        else if (falling && isGrounded) //If we have stopped falling and "falling" still true
        {
            animator.SetBool("isFalling", false);
            DoLandParticles(fallVelocityReached);
            SoundManager.Instance.PlayRandomSoundEffect(landSounds, transform, 2f);
            falling = false;
        }
    }
    
    //Called when the dash button is pressed
    private void Dash(InputAction.CallbackContext ctx)
    {
        if (!dashUnlocked || dashOnCooldown || isWallClimbing)
        {
            return;
        }

        if (!IsGrounded() && dashPerformed)
        {
            return;
        } //If we are not grounded and dash is already been performed then NO!

        rb.linearVelocity = Vector2.zero; //Reset velocity
        //Flags
        isDashing = true;
        dashPerformed = true;
        
        animator.SetBool("isDashing", true);
        SoundManager.Instance.PlayRandomSoundEffect(dashSounds, transform, 0.5f);

        DoDashParticles();

        StartCoroutine(DashTimings(dashTime, dashCooldown));
        StartCoroutine(DisableMovementFor(dashTime));
        rb.AddForceX(transform.right.x * dashForce, ForceMode2D.Impulse);
    }

    //Called when jump button pressed down
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isDashing) { JumpDashTech(); } //Jump dash tech!

        if (IsWallClimbing() && !isWallJumping && wallClimbingUnlocked)
        {
            PerformWallJump();
            return;
        }
        
        //Double Jump Code
        if (doubleJumpUnlocked && !doubleJumpPerformed)
        {
            PerformJump(doubleJumpForce);
            doubleJumpPerformed = true;
        }
        
        //Normal Jump Code
        if(!IsGrounded()) { return; } //Don't jump if alr in air
        if(movementDisabled) { return; }
        
        PerformJump(jumpForce);
    }

    private void PerformJump(float jumpForce)
    {
        //Jump
        rb.linearVelocityY = 0;
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        
        animator.SetTrigger("Jump");
        
        Instantiate(jumpParticles, groundPartcilesPos.position, Quaternion.identity);
        SoundManager.Instance.PlayRandomSoundEffect(jumpSounds, transform, 0.5f);
    }
    
    private void PerformWallJump()
    {
        if (!wallClimbingUnlocked) { return;}
        
        StartCoroutine(WallJumpTimer());
        StartCoroutine(DisableMovementFor(wallJumpTime));
        
        rb.linearVelocityY = 0; //Reset Velocity
        rb.gravityScale = baseGravity; // Reset gravity if already hasn't been
        
        rb.AddForceY(wallJumpForceY, ForceMode2D.Impulse);
        rb.AddForceX(-transform.right.x * wallJumpForceX, ForceMode2D.Impulse);
        
        SoundManager.Instance.PlayRandomSoundEffect(jumpSounds, transform, 0.5f);
    }

    //Called when jump button released
    private void CancelJump(InputAction.CallbackContext ctx)
    {
        if (movementDisabled) { return; }
        if (rb.linearVelocity.y > 0)
        {
            ApplyVerticalDamping();
        }
    }

    private void ApplyVerticalDamping()
    {
        rb.linearVelocityY *= (1/jumpReleaseDamping);
    }

    //Checks if the player is grounded when called
    private bool IsGrounded()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer)
            || Physics2D.OverlapCircle(coyoteJumpPos.position, coyoteJumpRadius, groundLayer))
        {
            doubleJumpPerformed = false; //Reset double jump
            dashPerformed = false;       //Reset dash
            return true;
        }
        return false;
    }
    
    private void Flip()
    {
        facingRight = !facingRight;
        LeanTween.rotateY(gameObject, facingRight ? 0 : 180, flipTime).setEaseInOutSine();
        ChangedLookDir?.Invoke(moveDirection);
    }
    
    private void DoDashParticles()
    {
        //Visuals
        if (transform.right.x < 0)
        {
            Instantiate(dashParticles, transform.position, Quaternion.identity);
        } 
        else {Instantiate(dashParticles, transform.position, new Quaternion(0f, -180f, 0f, 0f));}
    }

    public bool GetIsGrounded()
    {
        return IsGrounded();
    }
    
    private IEnumerator DisableMovementFor(float duration)
    {
        movementDisabled = true;
        yield return new WaitForSecondsRealtime(duration);
        movementDisabled = false;
    }

    private IEnumerator DashTimings(float dashTime, float dashCooldown)
    {
        yield return new WaitForSecondsRealtime(dashTime);
        isDashing = false;
        animator.SetBool("isDashing", false);
        StartCoroutine(DashCooldown(dashCooldown));
    }

    private IEnumerator DashCooldown(float duration)
    {
        dashOnCooldown = true;
        yield return new WaitForSecondsRealtime(duration);
        dashOnCooldown = false;
    }

    private IEnumerator WallJumpTimer()
    {
        isWallJumping = true;
        animator.SetBool("isWallJumping", isWallJumping);
        yield return new WaitForSecondsRealtime(wallJumpTime);
        isWallJumping = false;
        animator.SetBool("isWallJumping", isWallJumping);
    }

    private void JumpDashTech()
    {
        StopCoroutine("DashTimings");
        isDashing = false;
    }

    private bool IsWallClimbing()
    {
        return Physics2D.OverlapCircle(wallCheckPos.position, 0.2f, groundLayer) && !IsGrounded() && falling;
    }

    private void DoLandParticles(float fallVelocity)
    {
        GameObject particle = Instantiate(landParticles, groundPartcilesPos.position, Quaternion.identity);
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startSpeed = Mathf.Pow(Mathf.Abs(fallVelocity) ,landParticleSpeedExponent);
        main.startSize = (Mathf.Abs(fallVelocity) / maxVelocity) * landParticleSizeMultiplier;
    }

    private IEnumerator PlayWalkSounds()
    {
        SoundManager.Instance.PlayRandomSoundEffect(walkSounds, transform, 1);
        yield return new WaitForSeconds(0.2f);

        if (isWalking())
        {
            StartCoroutine(PlayWalkSounds());
        }
        else
        {
            isPlayingWalkSounds = false;
        }
    }

    private bool isWalking()
    {
        if (IsGrounded() && Math.Abs(rb.linearVelocityX) > 0.5f) {return true;}
        return false;
    }
    
}
