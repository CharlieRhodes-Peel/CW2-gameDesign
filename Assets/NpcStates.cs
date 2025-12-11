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
    
    private State currentState;

    private void Start()
    {
        SetCurrentState(defaultState);
    }

    public enum State
    {
        Neutral,
        Happy,
        Angry,
    }

    public State GetCurrentState()
    {
        return currentState;
    }

    //                                  --- Neutral States ---
    private void OnNeutralEnter()
    {
        feelingRenderer.sprite = neutralSprite;
        
        gameObject.layer = LayerMask.NameToLayer("Neutral");
    }

    private void OnNeutralExit()
    {
        
    }

    //                                  --- Happy States ---
    private void OnHappyEnter()
    {
        feelingRenderer.sprite = happySprite;

        gameObject.layer = LayerMask.NameToLayer("Happy");

        frogRenderer.sortingLayerID = SortingLayer.NameToID("Background");
        frogRenderer.sortingOrder = 1; //Puts it just above everything in the background

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
        
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        
        //Turn the face to angry
        feelingRenderer.sprite = angrySprite;
    }

    private void OnAngryExit()
    {
        interactDetector.SetActive(true);
        feelingDisplay.SetActive(false);
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
    
    //This is for the inspector, yeah it's boilerplate ik, but I need it!
    public static void SetStateToNeutral()
    {
        NpcStates callingInstance = InteractManager.GetClosestActor()?.GetComponent<NpcStates>();
        callingInstance?.SetCurrentState(State.Neutral);
    }
    public static void SetStateToHappy()
    {
        Debug.Log("Someone wants to be happy");
        NpcStates callingInstance = InteractManager.GetClosestActor().GetComponent<NpcStates>();
        Debug.Log(callingInstance.gameObject.name + " Wants to be happy");
        callingInstance.SetCurrentState(State.Happy);
        Debug.Log(callingInstance.gameObject.name + " Is now happy!");
    }
    public static void SetStateToAngry()
    {
        NpcStates callingInstance = InteractManager.GetClosestActor()?.GetComponent<NpcStates>();
        callingInstance?.SetCurrentState(State.Angry);
    }
}
