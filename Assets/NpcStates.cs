using System;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class NpcStates : MonoBehaviour
{
    [SerializeField] private State defaultState;

    [Header("States")] 
    [SerializeField] private SpriteRenderer feelingRenderer;

    [SerializeField] private SpriteRenderer frogRenderer;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite angrySprite;

    [Header("References")] 
    [SerializeField] private GameObject interactDetector;
    [SerializeField] private Transform aboveHeadPos;
    [SerializeField] private GameObject feelingDisplay;
    [SerializeField] private GameObject hitbox;
    
    private State currentState;
    private FrogMovement frogMovement;

    private void Start()
    {
        SetCurrentState(defaultState);
        frogMovement = GetComponent<FrogMovement>();
    }

    [Serializable]
    public enum State
    {
        Neutral = 0,
        Happy   = 1,
        Angry   = 2,
    }

    public State GetCurrentState()
    {
        return currentState;
    }

    //                                  --- Neutral States ---
    private void OnNeutralEnter()
    {
        feelingRenderer.sprite = neutralSprite;
        
        gameObject.tag = "Neutral";
        gameObject.layer = LayerMask.NameToLayer("Neutral");
        
        hitbox.SetActive(false);
    }

    private void OnNeutralExit()
    {
        
    }

    //                                  --- Happy States ---
    private void OnHappyEnter()
    {
        feelingRenderer.sprite = happySprite;

        gameObject.layer = LayerMask.NameToLayer("Happy");
        gameObject.tag = "Happy";

        frogRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        frogRenderer.sortingOrder = 1; //Puts it just above everything in the background

        
        hitbox.SetActive(false);
    }

    private void OnHappyExit()
    {
        feelingRenderer.sprite = happySprite;
        
        frogRenderer.sortingLayerID = SortingLayer.NameToID("Default"); //Restore Entry stuff
        frogRenderer.sortingOrder = 0;
    }
    
    //                                  --- Angry States ---
    private void OnAngryEnter()
    {
        //Get rid of the detector
        interactDetector.SetActive(false);
        
        //Put the indicator above the player
        feelingDisplay.SetActive(true);
        feelingDisplay.transform.position = aboveHeadPos.position;
        
        gameObject.tag = "Enemy";
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        
        //Turn the face to angry
        feelingRenderer.sprite = angrySprite;
        
        hitbox.SetActive(true);
        frogMovement.enabled = true;
    }

    private void OnAngryExit()
    {
        interactDetector.SetActive(true);
        feelingDisplay.SetActive(false);
        
        hitbox.SetActive(false);
    }
    
    //Changes states, called from outside
    public void SetCurrentState(State newState)
    {
        State oldState = currentState; //Just giving it a more understandable name
        
        //Perform Exit State functions
        switch (oldState)
        {
            case State.Neutral:
                OnNeutralExit(); break;
            case State.Happy:
                OnHappyExit(); break;
            case State.Angry:
                OnAngryExit(); break;
        }
        
        //Update to the new state
        currentState = newState;

        //Perform Enter State functions
        switch (currentState)
        {
            case State.Neutral:
                OnNeutralEnter(); break;
            case State.Happy:
                OnHappyEnter(); break;
            case State.Angry:
                OnAngryEnter(); break;
        }
    }

    public static void SetStateTo(string stateName)
    {
        NpcStates callingInstance = InteractManager.GetClosestActor().GetComponent<NpcStates>();
        callingInstance.SetCurrentState(stringToState(stateName)); 
    }


    private static State stringToState(string stateName)
    {
        switch (stateName)
        {
            case "Happy":
                return State.Happy;
            case "Angry":
                return State.Angry;
            default:
                return State.Neutral;
        }
    }
}
