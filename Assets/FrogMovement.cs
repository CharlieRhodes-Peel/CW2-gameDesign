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
    
    //Other scripts on this object
    private Rigidbody2D rb;
    private NpcStates npcStates;
        
    //Flags
    private bool jumping = false;
    private bool stop = false;
    private bool facingLeft = false;
    
    //Others
    private Transform playerPos; //Player gets found on scene load
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        npcStates = GetComponent<NpcStates>();
    }

    void OnEnable()
    {
        NpcActor.playerEnterRangeEvent += PlayerEnterRange;
        NpcActor.playerExitRangeEvent += PlayerExitRange;
        SceneSwitchManager.onSceneLoaded += FindPlayer;
    }

    void OnDisable()
    {
        NpcActor.playerEnterRangeEvent -= PlayerEnterRange;
        NpcActor.playerExitRangeEvent -= PlayerExitRange;
        SceneSwitchManager.onSceneLoaded -= FindPlayer;
    }

    private void Update()
    {
        //If in angry state
        if (npcStates.GetCurrentState() == NpcStates.State.Angry)
        {
            if (!playerPos.gameObject.activeInHierarchy) { return; } //Player is dead don't bother
            
            //Face the player
            bool shouldFaceLeft = playerPos.position.x - transform.position.x < 0;

            if (shouldFaceLeft && !facingLeft) { Flip(); }
            else if (!shouldFaceLeft && facingLeft) { Flip(); }
        }
        
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
        if (facingLeft) { transform.rotation = Quaternion.Euler(0f, -180f, 0f); } //Then face right
        else { transform.rotation = Quaternion.Euler(0f, 0f, 0f); } //Face left
        
        facingLeft = !facingLeft;
    }

    //Called whenever the player enters our range
    private void PlayerEnterRange(GameObject caller)
    {
        if (caller != this.gameObject) {return;}
        
        stop = true;
    }

    //Look at the function above and think about when this one gets called... it's the same
    private void PlayerExitRange(GameObject caller)
    {
        if (caller != this.gameObject) { return; }
        
        stop = false;
    }
    
    //Called when the scene loads
    private void FindPlayer()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
