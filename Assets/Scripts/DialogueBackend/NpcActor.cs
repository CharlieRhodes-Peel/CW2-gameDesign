//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
 
public class NpcActor : MonoBehaviour, IInteractable
{
    public string Name;

    public List<Dialogue> DialogueTrees;
    
    [SerializeField] private Transform popupPos; //Determines where the popup prompt will appear
    [SerializeField] private string popupText;
    [SerializeField] private GameObject feeling;
    
    [SerializeField] private bool givesMoney;
    [ShowIf("givesMoney")] [SerializeField] private GameObject moneySpawnerPrefab;
    [SerializeField] private bool givesItem;
    [ShowIf("givesItem")] [SerializeField] private Transform itemGivePos;
    

    private bool playerInRange = false;
    private bool facingLeft = true;
    
    private Transform playerTransformOnEnter;
    private NpcStates npcStates;
    private bool usingNpcStates = false;
    
    public static event Action<GameObject> playerEnterRangeEvent; //Called to let other scripts know player is in range of US
    public static event Action<GameObject> playerExitRangeEvent; //Called to let other scripts know player is OUT of range of us

    public static event Action<string> OnPlayerInteractEvent; //Called when the player interacts with us to let other scripts know the name of the character we are talking to

    private bool activeQuest = false;
    private bool spokenTo = false;
    
    private void Start()
    {
        npcStates = GetComponent<NpcStates>();
        
        if (npcStates == null) {usingNpcStates = false;}
        else
        {
            usingNpcStates = true;
            feeling.SetActive(false);
        }
        
        //Check for any active quest
        activeQuest = QuestManager.Instance.HasQuestWithNPC(Name);
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
            DialogueManager.Instance.StartDialogue(Name, QuestManager.Instance.GetActiveDialogue(Name).RootNode);
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
        
        playerEnterRangeEvent?.Invoke(gameObject);
        
        InteractManager.RegisterInteractable(this);
        NpcTracker.RegisterActor(this);
        
        NpcActorNameManager.UnregisterActor(this);
        
        if (usingNpcStates) {feeling.SetActive(true);}
    }

    //Called when the player leaves the speaking range
    public void PlayerExitSpeakingRange(Transform playerTransform)
    {
        playerInRange = false;

        playerExitRangeEvent?.Invoke(gameObject);
        
        InteractManager.UnregisterInteractable(this);
        NpcTracker.UnregisterActor(this);
        
        if (spokenTo) {NpcActorNameManager.RegisterActor(this);}

        //If we're not angry then when we leave disable the indicator
        if (!usingNpcStates) { return; }
        if (npcStates.GetCurrentState() != NpcStates.State.Angry)
        {
            feeling.SetActive(false);
        }
    }
    
    //Called when the player interact with them :)
    public void Interact()
    {
        OnPlayerInteractEvent?.Invoke(Name);
        
        SpeakTo();
        spokenTo = true;
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
        NpcTracker.GetClosestNpcActor().activeQuest = true;
        QuestManager.Instance.StartQuest(quest);
    }
    
    public static void FinishQuest(Quest quest)
    {
        NpcTracker.GetClosestNpcActor().activeQuest = false;
        QuestManager.Instance.FinishQuest(quest);
    }

    private void SpawnMoney(int amount)
    {
        if (amount < 1) {return;}
        if (moneySpawnerPrefab == null) {return;}

        MoneySpawner moneySpawnerInScene = Instantiate(moneySpawnerPrefab, transform.position, Quaternion.identity).GetComponent<MoneySpawner>();
        moneySpawnerInScene.moneyToSpawn = amount;
        moneySpawnerInScene.Spawn();
    }
    
    public static void SpawnMoneyOnMe(int amount)
    {
        NpcTracker.GetClosestNpcActor().SpawnMoney(amount);
    }

    public static void SpawnItemOnMe(GameObject item)
    {
        NpcTracker.GetClosestNpcActor().SpawnItem(item);
    }

    private void SpawnItem(GameObject item)
    {
        Instantiate(item, itemGivePos.position, Quaternion.identity);
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