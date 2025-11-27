using TMPro;
using UnityEngine;
 
public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;
    
    [SerializeField] private Transform popupPos; //Determines where the popup prompt will appear
    
    // Trigger dialogue for this actor
    public void SpeakTo()
    {
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteract.PlayerInteractWith += PlayerTalkedToMe; //Subscribe to interact event from player
            InteractManager.TellPlayerIWantThem(this); //Tells the manager that I want to talk to the player
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteract.PlayerInteractWith -= PlayerTalkedToMe; //Unsubscribe from player interact event so we are not spammed!
            InteractManager.TellPlayerIDontWantThem(this); //Tell the manager that we don't want to interact with the player anymore
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
}