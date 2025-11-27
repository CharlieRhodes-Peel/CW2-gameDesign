using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractManager : MonoBehaviour
{
    private static List<Actor> actorsWantingToInteract = new List<Actor>();
    [SerializeField] private TextMeshProUGUI popupText;
    private Vector3 popUpPos;

    private static Actor closestActor;

    [SerializeField] private Transform playerPos;
    
    //Events
    public static event Action<Actor> InteractWithMePlayer;
    public static event Action<Actor> DontInteractWithMePlayer;
    

    // Update is called once per frame
    void Update()
    {
        closestActor = null; //This is so it doesn't get stuck
        if (actorsWantingToInteract.Count < 1) { DisablePopUp(); return; } //If there are no actors near player disable and move on
        EnablePopUp();
        
        closestActor = FindClosestActor();
        
        //Places text by the closest actor
        PlaceText(closestActor.GetPopupPos());
    }
    
    private Actor FindClosestActor()
    {
        Actor closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (Actor actor in actorsWantingToInteract)
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
    public static void TellPlayerIWantThem(Actor actor)
    {
        actorsWantingToInteract.Add(actor);

        if (actor == closestActor) { InteractWithMePlayer?.Invoke(actor); } //Tells player this is their closest actor
    }

    //Gets called on the frame an actor doesn't want to interact with the player anymore
    public static void TellPlayerIDontWantThem(Actor actor)
    {
        actorsWantingToInteract.Remove(actor);
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
    
    public static Actor GetClosestActor()
    {
        return closestActor;
    }
}
