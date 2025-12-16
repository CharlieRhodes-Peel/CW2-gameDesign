using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcActorNameManager : MonoBehaviour
{
    public static NpcActorNameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI actorNameText;
    
    private static List<NpcActor> actorsTalkedTo = new List<NpcActor>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {Destroy(gameObject);}
    }
    
    void Update()
    {
        if (actorsTalkedTo.Count < 1)
        {
            actorNameText.enabled = false;
            return;
        }
        
        NpcActor closestActor = ClosestNpcTracker.GetClosestNpcActor();
        if (closestActor == null)
        {
            actorNameText.enabled = false;
            return;
        }
        
        foreach (NpcActor actor in actorsTalkedTo)
        {
            if (actor == closestActor)
            {
                actorNameText.enabled = true;
                actorNameText.text = actor.Name;
                PlaceText(actor.GetInteractPopupPosition());
                return;
            }
        }
        
        actorNameText.enabled = false; //Safeguard
    }
    
    private void PlaceText(Vector3 pos)
    {
        actorNameText.transform.position = Camera.main.WorldToScreenPoint(pos);
    }

    public static void RegisterActor(NpcActor actor)
    {
        actorsTalkedTo.Add(actor);
    }

    public static void UnregisterActor(NpcActor actor)
    {
        actorsTalkedTo.Remove(actor);
    }
}
