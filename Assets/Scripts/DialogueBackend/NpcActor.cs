//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
 
public class NpcActor : MonoBehaviour
{
    public string Name;

    public List<Dialogue> DialogueTrees;
    
    [SerializeField] private Transform popupPos; //Determines where the popup prompt will appear
    [SerializeField] private GameObject feeling;

    private bool playerInRange = false;
    private bool facingLeft = true;
    
    private Transform playerTransformOnEnter;
    private NpcStates npcStates;
    
    public static event Action<GameObject> playerEnterRangeEvent; //Called to let other scripts know player is in range of US
    public static event Action<GameObject> playerExitRangeEvent; //Called to let other scripts know player is OUT of range of us

    private static bool activeQuest = false;
    
    private void Start()
    {
        npcStates = GetComponent<NpcStates>();
    }

    // Trigger dialogue for this actor
    public void SpeakTo()
    {
        PickDialogueTree();
    }

    private void PickDialogueTree()
    {
        //If the player has an active quest with the NPC it takes precedent over any dialogue trees
        if (activeQuest)
        {
            //Do the quest Dialogue
            DialogueManager.Instance.StartDialogue(Name, QuestManager.Instance.GetActiveDialogue().RootNode);
            return;
        }
        
        //Pick the first option in the list who's conditional is met
        bool dialoguePicked = false;
        foreach (Dialogue dialogue in DialogueTrees)
        {
            if (DialogueStateManager.instance.IsDialogueActive(dialogue))
            {
                dialoguePicked = true;
                DialogueManager.Instance.StartDialogue(Name, dialogue.RootNode);
                break;
            }
        }

        if (!dialoguePicked)
        {
            DialogueManager.Instance.StartDialogue(Name, DialogueTrees[0].RootNode); //If no dialogue conditional default to the start one
        }
    }

    public void Update()
    {
        if (playerInRange)
        {
            FacePlayer();
        }
    }

    //Called when the player gets into speaking range
    public void PlayerEnterSpeakingRange(Transform playerTransform)
    {
        playerInRange = true;
        playerTransformOnEnter = playerTransform;
            
        PlayerInteract.PlayerInteractWith += PlayerTalkedToMe; //Subscribe to interact event from player
        NpcInteractManager.TellPlayerIWantThem(this); //Tells the manager that I want to talk to the player
        
        playerEnterRangeEvent?.Invoke(gameObject);
        
        feeling.SetActive(true);
    }

    //Called when the player leaves the speaking range
    public void PlayerExitSpeakingRange(Transform playerTransform)
    {
        playerInRange = false;
        
        PlayerInteract.PlayerInteractWith -= PlayerTalkedToMe; //Unsubscribe from player interact event so we are not spammed!
        NpcInteractManager.TellPlayerIDontWantThem(this); //Tell the manager that we don't want to interact with the player anymore

        playerExitRangeEvent?.Invoke(gameObject);

        //If we're not angry then when we leave disable the indicator
        if (npcStates.GetCurrentState() != NpcStates.State.Angry)
        {
            feeling.SetActive(false);
        }
    }

    //Triggered when the player has talked to me
    private void PlayerTalkedToMe(NpcActor npcActor)
    {
        if (npcActor != this) { return; }
        
        SpeakTo();
    }
    
    public Vector3 GetPopupPos()
    {
        return popupPos.position;
    }

    private void FacePlayer()
    {
        if (playerTransformOnEnter.position.x - transform.position.x > 0 && facingLeft) { Flip(); }
        else if (playerTransformOnEnter.position.x - transform.position.x < 0 && !facingLeft) { Flip(); }
    }

    private void Flip()
    {
        if (facingLeft) { transform.rotation = Quaternion.Euler(0f, -180f, 0f); }
        else { transform.rotation = Quaternion.Euler(0f, 0f, 0f); }
        
        facingLeft = !facingLeft;
    }
    
    //This is for unity events
    public static void MakeDialogueInactive(Dialogue dialogue)
    { DialogueStateManager.instance.MakeDialogueInActive(dialogue); }

    public static void MakeDialogueActive(Dialogue dialogue)
    { DialogueStateManager.instance.MakeDialogueActive(dialogue); }

    public static void StartQuest(Quest quest)
    {
        activeQuest = true;
        QuestManager.Instance.StartQuest(quest);
    }
    
    public static void FinishQuest(Quest quest)
    {
        activeQuest = false;
        QuestManager.Instance.QuestComplete(quest);
        QuestManager.Instance.QuestExitProcessing(quest);
    }
    
    public static void QuestExitProcessing(Quest quest)
    { QuestManager.Instance.QuestExitProcessing(quest); }
}