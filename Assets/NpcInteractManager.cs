using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcInteractManager : MonoBehaviour
{
    private static List<NpcActor> npcsWantingToInteract = new List<NpcActor>();
    [SerializeField] private TextMeshProUGUI popupText;
    private Vector3 popUpPos;

    private static NpcActor closestNpcActor;

    [SerializeField] private Transform playerPos;
    
    //Events
    public static event Action<NpcActor> InteractWithMePlayer;
    public static event Action<NpcActor> DontInteractWithMePlayer;

    private void Start()
    {
        StartCoroutine(WaitForTxtMeshProBug());
    }

    // Update is called once per frame
    void Update()
    {
        if (npcsWantingToInteract.Count < 1) //If there are no actors near player disable and move on
        { 
            DisablePopUp();
            return;
        }
        EnablePopUp();
        
        closestNpcActor = FindClosestActor();
        
        //Places text by the closest actor
        PlaceText(closestNpcActor.GetPopupPos());
    }
    
    private NpcActor FindClosestActor()
    {
        NpcActor closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (NpcActor actor in npcsWantingToInteract)
        {
            float distance = Vector2.Distance(actor.gameObject.transform.position, playerPos.position);

            if (distance < closestDistance)
            {
                closest = actor; 
                closestDistance = distance;
            }
        }
        return closest;
    }
    
    //Gets called on the frame an actor wants to interact with the player
    public static void TellPlayerIWantThem(NpcActor npcActor)
    {
        npcsWantingToInteract.Add(npcActor);

        if (npcActor == closestNpcActor) { InteractWithMePlayer?.Invoke(npcActor); } //Tells player this is their closest actor
    }

    //Gets called on the frame an actor doesn't want to interact with the player anymore
    public static void TellPlayerIDontWantThem(NpcActor npcActor)
    {
        npcsWantingToInteract.Remove(npcActor);
    }
    
    private void PlaceText(Vector3 pos)
    {
        popupText.transform.position = Camera.main.WorldToScreenPoint(pos);
    }

    private void DisablePopUp()
    {
        popupText.gameObject.SetActive(false);
    }

    private void EnablePopUp()
    {
        popupText.gameObject.SetActive(true);
    }
    
    public static NpcActor GetClosestActor()
    {
        return closestNpcActor;
    }

    //Called at the start of the game because Text Mesh Pro's Awake() function is a 256kb garbage collection NUKE
    //So call it at the start of the game to kinda "preload it" so that it doesn't happen during gameplay
    private IEnumerator WaitForTxtMeshProBug()
    {
        popupText.gameObject.SetActive(true);
        yield return new WaitForFixedUpdate();
        popupText.gameObject.SetActive(false);
    }
}
