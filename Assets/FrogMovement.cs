using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float jumpInterval;
    [SerializeField] private float jumpForceY;
    [SerializeField] private float jumpForceX;
    
    [Header("References")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Collider2D behindCollider;
    
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private Animator animator;
    
    private Rigidbody2D rb;
    
    private bool jumping = false;
    private bool stop = false;
    private bool flipFlag = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        Actor.playerEnterRangeEvent += PlayerEnterRange;
        Actor.playerExitRangeEvent += PlayerExitRange;
    }

    void OnDisable()
    {
        Actor.playerEnterRangeEvent -= PlayerEnterRange;
        Actor.playerExitRangeEvent -= PlayerExitRange;
    }

    private void Update()
    {
        if  (hitWallCheck()) {Flip();}
    }
    
    private void FixedUpdate()
    {
        if (stop) { StopAllCoroutines(); }
        else if (jumping) { return; }

        StartCoroutine(Jump());
    }
    
    private IEnumerator Jump()
    {
        jumping = true;
        yield return new WaitForSeconds(jumpInterval);
        
        //Actually jump
        animator.SetTrigger("Jump");
        rb.AddForce(Vector2.up * jumpForceY, ForceMode2D.Impulse);
        rb.AddForce(-transform.right * jumpForceX, ForceMode2D.Impulse);
        jumping = false;
    }

    private bool hitWallCheck()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, whatIsWall);
    }

    private void Flip()
    {
        if (flipFlag) { transform.rotation = Quaternion.Euler(0f, -180f, 0f); }
        else { transform.rotation = Quaternion.Euler(0f, 0f, 0f); }
        
        flipFlag = !flipFlag;
    }

    //Called whenever the player enters our range
    private void PlayerEnterRange(GameObject caller)
    {
        if (caller != this.gameObject) {return;}
        
        stop = true;
    }

    private void PlayerExitRange(GameObject caller)
    {
        if (caller != this.gameObject) { return; }
        
        stop = false;
    }
}
