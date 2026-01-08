using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] private float hitboxActiveTime;
    [SerializeField] private float attackWindup;
    [SerializeField] private float cooldownPostHitbox;
    [SerializeField] private float downAttackBounceForce;

    [Header("Enemy Affectors")] 
    [SerializeField] private float damagePerHit;

    [Header("References")] 
    [SerializeField] private GameObject sideAttackHitBox;
    [SerializeField] private GameObject upAttackHitBox;
    [SerializeField] private GameObject downAttackHitBox;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private InputActionReference movingInput;
    
    [Header("Visuals")]
    [SerializeField] private Animator animator;

    [Header("Sounds")]
    [SerializeField] private AudioClip[] swingSounds;
    [SerializeField] private AudioClip[] hitSounds;
    
    private Vector2 movementInput;

    //Privates
    private bool isAttacking;
    private bool isWindup = false;
    private GameObject currentHitbox;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    [HideInInspector] public bool attackDisabled = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    //Get player input
    private void Update()
    {
        movementInput = movingInput.action.ReadValue<Vector2>(); 
    }

    
    private void FixedUpdate()
    {
        //Dont change the hitbox is we're already attacking!
        if (isAttacking) {return;}
        
        //Depending on where the player is looking determines attack
        if (movementInput.y > 0)                                         { currentHitbox = upAttackHitBox; }
        else if (movementInput.y < 0 && !playerMovement.GetIsGrounded()) { currentHitbox = downAttackHitBox; }
        else                                                             { currentHitbox = sideAttackHitBox;}
    }

    //Called whenever the attack button is pressed
    private void Attack(InputAction.CallbackContext ctx)
    {
        Debug.Log("AttackButtonPressed");
        if (attackDisabled) { return; }
        Debug.Log("Attack is not disbaled");
        if (isAttacking) {return;}
        Debug.Log("is attack is false!");
        if (playerMovement.isWallClimbing) {return;}
        Debug.Log("Player is not wall climbing!");
        
        StartCoroutine(StartAttacking());
    }

    private IEnumerator StartAttacking()
    {
        isAttacking = true;
        isWindup = true;
        StartCoroutine(SetAnimatorCorrectAttack());
        SoundManager.Instance.PlayRandomSoundEffect(swingSounds, transform, 1);
        
        yield return new WaitForSeconds(attackWindup);
        
        currentHitbox.SetActive(true);
        yield return new WaitForSeconds(hitboxActiveTime);
        currentHitbox.SetActive(false);
        isWindup = false;
        
        yield return new WaitForSeconds(cooldownPostHitbox);
        isAttacking = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Neutral"))
        {
            other.GetComponent<Enemy>().TakeDamage(damagePerHit, transform.position);
            SoundManager.Instance.PlayRandomSoundEffect(hitSounds, transform, 1);

            if (currentHitbox == downAttackHitBox) //If we are attacking down when we hit the enemy we want to bounce off them
            {
                rb.linearVelocityY = 0; //Reset current linearVelocity
                rb.AddForceY(downAttackBounceForce,  ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator SetAnimatorCorrectAttack()
    {
        if (currentHitbox == downAttackHitBox)
        {
            animator.SetTrigger("attackDown");
        }
        else if (currentHitbox == sideAttackHitBox)
        {
            animator.SetTrigger("attackSide");
        }
        else if (currentHitbox == upAttackHitBox)
        {
            animator.SetTrigger("attackUp");
        }
        yield return new WaitForFixedUpdate();
        resetAnimationTriggers();
    }

    private void resetAnimationTriggers()
    {
        animator.ResetTrigger("attackDown");
        animator.ResetTrigger("attackSide");
        animator.ResetTrigger("attackUp");
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        isAttacking = false;
        isWindup = false; 
    }


    public bool IsWindup() { return isWindup; }

    public void AddToDamagePerHit(float damage)
    {
        damagePerHit += damage;
    }
    
    //Subscribes and Unsubscribes to input events when player exists or doesn't
    private void OnEnable()
    {
        attackInput.action.Enable();
        attackInput.action.started += Attack;
    }

    private void OnDisable()
    {
        attackInput.action.started -= Attack;
        attackInput.action.Disable();
    }

    public void resetAttackStuff()
    {
        isAttacking = false;
        isWindup = false;
        attackDisabled = false;
    }
}
