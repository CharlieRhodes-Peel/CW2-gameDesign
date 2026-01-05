using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class FrogMovement : MonoBehaviour
{
    [Header("Movement Types")]
    [SerializeField] private MovementType movementType;
    
    //Walking
    [ShowIf("movementType", MovementType.WalkOnly)] [SerializeField] private float walkSpeed;

    [SerializeField] private bool walkingBetweenPoints = false;
    [SerializeField] private bool wallMakesFrogTurnAround = false;
    [ShowIf("wallMakesFrogTurnAround")] [SerializeField] private Transform wallCheck;
    
    //Jumping
    [SerializeField] private float jumpInterval;
    [SerializeField] private float jumpForceY;
    [SerializeField] private float jumpForceX;
    
    [Header("References")]
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

    public enum MovementType
    {
        WalkOnly,
        JumpOnly,
        WalkAndJump
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        npcStates = GetComponent<NpcStates>();
        SceneSwitchManager.onSceneLoaded += FindPlayer;
    }

    void OnEnable()
    {
        NpcActor.playerEnterRangeEvent += PlayerEnterRange;
        NpcActor.playerExitRangeEvent += PlayerExitRange;
        
        //Find the player when the script is enabled
        FindPlayer();
    }

    void OnDisable()
    {
        NpcActor.playerEnterRangeEvent -= PlayerEnterRange;
        NpcActor.playerExitRangeEvent -= PlayerExitRange;
    }

    private void Update()
    {
        NpcStates.State currentState = npcStates.GetCurrentState();
        //If in angry state
        if (currentState == NpcStates.State.Angry)
        {
            if (!playerPos.gameObject.activeInHierarchy) { return; } //Player is dead don't bother

            FacePlayer();
        }
        

        if (hitWallCheck())
        {
            if (movementType == MovementType.WalkOnly && currentState != NpcStates.State.Angry)
            {
                Flip();
            }
            
            if (movementType == MovementType.WalkOnly && !jumping && currentState == NpcStates.State.Angry)
            {
                StartCoroutine(Jump());
            }
            else if (movementType == MovementType.JumpOnly)
            {
                Flip();
            }
        }
    }

    private void FacePlayer()
    {
        //Face the player
        bool shouldFaceLeft = playerPos.position.x - transform.position.x < 0;

        if (shouldFaceLeft && !facingLeft) { Flip(); }
        else if (!shouldFaceLeft && facingLeft) { Flip(); }
    }

    private void FixedUpdate()
    {
        animator.SetFloat("moveSpeed", Mathf.Abs(rb.linearVelocityX));
        
        if (stop) { StopAllCoroutines(); return; }
        
        //Jump only
        if (movementType == MovementType.JumpOnly)
        {
            if (jumping) { return; }
            StartCoroutine(Jump());
        }
        
        //Walk
        else if (movementType == MovementType.WalkOnly)
        {
            rb.linearVelocityX = -transform.right.x * walkSpeed;
        }
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
        if (!wallMakesFrogTurnAround) { return false; }
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
        FacePlayer();
    }

    //Look at the function above and think about when this one gets called... it's the same
    private void PlayerExitRange(GameObject caller)
    {
        if (caller != this.gameObject) { return; }
        
        stop = false;
        FacePlayer();
    }
    
    //Walking between points
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (npcStates.GetCurrentState() == NpcStates.State.Angry) { return; }
        if (!walkingBetweenPoints) { return;}

        if (other.CompareTag("PathfindingPoint"))
        {
            Flip();
        }
    }

    //Called when the scene loads
    private void FindPlayer()
    {
        if (playerPos != null) { return; }
        
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerPos == null) { StartCoroutine(LookForPlayerAgain());
            return;
        }
        
        Debug.Log($"I think I found player! He is at {playerPos}");
    }

    private IEnumerator LookForPlayerAgain()
    {
        yield return new WaitForSecondsRealtime(1f);
        FindPlayer();
    }

    private void OnDestroy()
    {
        SceneSwitchManager.onSceneLoaded -= FindPlayer;
    }

    public void SetStopTo(bool stop)
    {
        this.stop = stop;
    }
}
