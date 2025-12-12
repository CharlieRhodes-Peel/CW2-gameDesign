//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
using System;
using TMPro;
using UnityEngine;
 
public class NpcActor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;
    public Quest Quest;
    
    [SerializeField] private Transform popupPos; //Determines where the popup prompt will appear
    [SerializeField] private GameObject feeling;

    private bool playerInRange = false;
    private bool facingLeft = true;
    
    private Transform playerTransformOnEnter;
    private NpcStates npcStates;
    
    public static event Action<GameObject> playerEnterRangeEvent; //Called to let other scripts know player is in range of US
    public static event Action<GameObject> playerExitRangeEvent; //Called to let other scripts know player is OUT of range of us

    private void Start()
    {
        npcStates = GetComponent<NpcStates>();
    }

    // Trigger dialogue for this actor
    public void SpeakTo()
    {
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
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
        InteractManager.TellPlayerIWantThem(this); //Tells the manager that I want to talk to the player
        
        playerEnterRangeEvent?.Invoke(gameObject);
        
        feeling.SetActive(true);
    }

    //Called when the player leaves the speaking range
    public void PlayerExitSpeakingRange(Transform playerTransform)
    {
        playerInRange = false;
        
        PlayerInteract.PlayerInteractWith -= PlayerTalkedToMe; //Unsubscribe from player interact event so we are not spammed!
        InteractManager.TellPlayerIDontWantThem(this); //Tell the manager that we don't want to interact with the player anymore

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
        PlayerInteract.PlayerInteractWith -= PlayerTalkedToMe; //Unsubscribe from player interact so they cannot interact with us while talking!
        InteractManager.TellPlayerIDontWantThem(this); //Can no longer interact with NPC
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
}