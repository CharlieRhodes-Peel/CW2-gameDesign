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
    [SerializeField] private float knockbackPerHit;

    [Header("References")] 
    [SerializeField] private GameObject sideAttackHitBox;
    [SerializeField] private GameObject upAttackHitBox;
    [SerializeField] private GameObject downAttackHitBox;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private InputActionReference movingInput;
    
    [Header("Visuals")]
    [SerializeField] private Animator animator;
    
    private Vector2 movementInput;

    //Privates
    private bool isAttacking;
    private GameObject currentHitbox;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

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
        if (isAttacking) {return;}
        
        StartCoroutine(StartAttacking());
    }

    private IEnumerator StartAttacking()
    {
        isAttacking = true;
        StartCoroutine(SetAnimatorCorrectAttack());
        
        yield return new WaitForSeconds(attackWindup);
        
        currentHitbox.SetActive(true);
        yield return new WaitForSeconds(hitboxActiveTime);
        currentHitbox.SetActive(false);
        
        yield return new WaitForSeconds(cooldownPostHitbox);
        isAttacking = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damagePerHit,  knockbackPerHit, transform.position);

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


    //Subscribes and Unsubscribes to input events when player exists or doesn't
    private void OnEnable()
    {
        attackInput.action.started += Attack;
    }

    private void OnDisable()
    {
        attackInput.action.started -= Attack;
    }
}
