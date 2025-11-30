using System;
using TMPro;
using UnityEngine;
 
public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;
    
    [SerializeField] private Transform popupPos; //Determines where the popup prompt will appear

    private bool playerInRange = false;
    private bool facingLeft = true;
    
    private Transform playerTransformOnEnter;
    
    public static event Action<GameObject> playerEnterRangeEvent; //Called to let other scripts know player is in range of US
    public static event Action<GameObject> playerExitRangeEvent; //Called to let other scripts know player is OUT of range of us
    
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransformOnEnter = other.transform;
            
            PlayerInteract.PlayerInteractWith += PlayerTalkedToMe; //Subscribe to interact event from player
            InteractManager.TellPlayerIWantThem(this); //Tells the manager that I want to talk to the player
            
            playerEnterRangeEvent?.Invoke(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            PlayerInteract.PlayerInteractWith -= PlayerTalkedToMe; //Unsubscribe from player interact event so we are not spammed!
            InteractManager.TellPlayerIDontWantThem(this); //Tell the manager that we don't want to interact with the player anymore
            
            playerExitRangeEvent?.Invoke(gameObject);
        }
    }

    //Triggered when the player has talked to me
    private void PlayerTalkedToMe(Actor actor)
    {
        if (actor != this) { return; }
        
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