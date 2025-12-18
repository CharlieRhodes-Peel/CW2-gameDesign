using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class LogBossStates : MonoBehaviour
{
    public static LogBossStates instance; //There is only ONE log boss!
    
    [SerializeField] private State defaultState = State.Peaceful;
    private State currentState;
    
    [Header("StateTimings")]
    [MinMaxSlider(0,5)] [SerializeField] private Vector2 idleTimeRange; //x is min y is max
    [MinMaxSlider(0,5)] [SerializeField] private Vector2 walkTimeRange;
    //[MinMaxSlider(0,20)] [SerializeField] private Vector2 rollTimeRange;
    //[MinMaxSlider(0,20)] [SerializeField] private Vector2 slamTimeRange;
    
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NpcActor npcActor;
    [SerializeField] private GameObject speakRange;
    
    public enum State
    {
        Peaceful     = 0,           //When the player is not fighting them
        Idle         = 1,           //The rest dictate fighting behaviour
        WalkToPlayer = 2,
        RollAttack   = 3,
        SlamAttack   = 4
    }

    //Ensures there is only one instance of log boss running
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else {Destroy(gameObject);}
    }

    private void Start()
    {
        SetCurrentState(defaultState);
    }
    
    //State management!
    
    // -- Peaceful --
    private void OnPeacefulEnter() 
    {
        animator.enabled = false;
        npcActor.enabled = true;
        speakRange.SetActive(true);
        
    }
    private void OnPeacefulExit()
    { 
        animator.enabled = true;
        npcActor.enabled = false;
        speakRange.SetActive(false);
    }
    
    // -- Idle -- 
    
    private void OnIdleEnter()
    {
        animator.SetBool("isIdle", true);
        float stayTime = Random.Range(idleTimeRange.x, idleTimeRange.y);
        State nextState = GetRandomAttackFightState();
        
        StartCoroutine(WaitForNextState(nextState, stayTime));
    }
    private void OnIdleExit()
    {
        animator.SetBool("isIdle", false);
    }
    
    // -- WalkToPlayer --
    
    private void OnWalkToPlayerEnter()
    {
        float stayTime = Random.Range(walkTimeRange.x, walkTimeRange.y);
        State nextState = GetRandomAttackFightState();
        
        StartCoroutine(WaitForNextState(nextState, stayTime));
    }

    private void OnWalkToPlayerExit()
    {
    }
    
    // -- RollAttack --
    
    private void OnRollAttackEnter()
    {
        //Animator calls change state at end of animation
        animator.SetTrigger("RollAttack");
    }
    
    private void OnRollAttackExit()
    {
    }

    // -- SlamAttack --
    
    private void OnSlamAttackEnter()
    {
        //Animator controls when state will end
        animator.SetTrigger("SlamAttack");
    }

    private void OnSlamAttackExit()
    {
    }



    private void SetCurrentState(State newState)
    {
        State oldState = currentState; //Giving it a more understandable name

        switch (oldState)
        {
            case State.Peaceful:
                OnPeacefulExit(); break;
            case State.Idle:
                OnIdleExit(); break;
            case State.WalkToPlayer:
                OnWalkToPlayerExit(); break;
            case State.RollAttack:
                OnRollAttackExit(); break;
            case State.SlamAttack:
                OnSlamAttackExit(); break;
        }
        
        //Update to new state
        currentState = newState;

        switch (currentState)
        {
            case State.Peaceful:
                OnPeacefulEnter(); break;
            case State.Idle:
                OnIdleEnter(); break;
            case State.WalkToPlayer:
                OnWalkToPlayerEnter(); break;
            case State.RollAttack:
                OnRollAttackEnter(); break;
            case State.SlamAttack:
                OnSlamAttackEnter(); break;
        }
    }

    private State GetRandomFightState()
    {
        return (State)Random.Range(1, 5);
    }

    private State GetRandomIdleFightState()
    {
        return (State)Random.Range(1, 3);
    }

    private State GetRandomAttackFightState()
    {
        return (State)Random.Range(3, 5);
    }

    private IEnumerator WaitForNextState(State newState, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        SetCurrentState(newState);
    }


    //For Unity Event  (called from dialogue)
    public static void StartFight()
    {
        instance.SetCurrentState(State.Idle);
    }

    //Called anytime an attack animation is complete
    public void OnAttackAnimationFinished()
    {
        //Go to an idle state
        Debug.Log("I should go to an idle state");
        State nextState = GetRandomIdleFightState();
        SetCurrentState(nextState);
    }

}
