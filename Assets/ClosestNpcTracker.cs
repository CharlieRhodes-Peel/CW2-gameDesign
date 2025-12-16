using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClosestNpcTracker : MonoBehaviour
{
    private static List<NpcActor> npcCloseEnoughToInterect = new List<NpcActor>();
    
    public Transform playerPos;
    
    private static NpcActor closestNpcActor;
    

    // Update is called once per frame
    void Update()
    {
        if (!playerPos.gameObject.activeInHierarchy) { return; }
        
        if (npcCloseEnoughToInterect.Count < 1) //If there are no actors near player disable and move on
        { 
            return;
        }
        
        closestNpcActor = FindClosestActor();
    }
    
    private NpcActor FindClosestActor()
    {
        NpcActor closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (NpcActor actor in npcCloseEnoughToInterect)
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

    public static void RegisterActor(NpcActor actor)
    {
        npcCloseEnoughToInterect.Add(actor);
    }

    public static void UnregisterActor(NpcActor actor)
    {
        npcCloseEnoughToInterect.Remove(actor);
    }

    public static NpcActor GetClosestNpcActor()
    {
        return closestNpcActor;
    }
}
