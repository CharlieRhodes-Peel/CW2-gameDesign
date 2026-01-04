//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
 
public class Statue : MonoBehaviour, IInteractable
{
    public string Name;

    public List<Dialogue> DialogueTrees;
    
    [SerializeField] private Transform popupPos; //Determines where the popup prompt will appear
    [SerializeField] private string popupText;

    private bool playerInRange = false;
    
    private Transform playerTransformOnEnter;
    private NpcStates npcStates;
    private bool usingNpcStates = false;


    private static bool activeQuest = false;
    private static bool spokenTo = false;
    
    private void Start()
    {
        npcStates = GetComponent<NpcStates>();
        
        if (npcStates == null) {usingNpcStates = false; return; }
        usingNpcStates = true;
    }

    // Trigger dialogue for this actor
    public void SpeakTo()
    {
        PickDialogueTree();
    }

    private void PickDialogueTree()
    {
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

    //Called when the player gets into speaking range
    public void PlayerEnterSpeakingRange(Transform playerTransform)
    {
        playerInRange = true;
        playerTransformOnEnter = playerTransform;
    }

    //Called when the player leaves the speaking range
    public void PlayerExitSpeakingRange(Transform playerTransform)
    {
        playerInRange = false;
    }
    
    //Called when the player interact with them :)
    public void Interact()
    {
        SpeakTo();
        spokenTo = true;
        
        //Don't let interact
        InteractManager.UnregisterInteractable(this);
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
        QuestManager.Instance.FinishQuest(quest);
    }
    
    

    public Vector3 GetInteractPopupPosition()
    {
        return popupPos.position;
    }

    public string GetInteractPopupText()
    {
        return popupText;
    }
}
