using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcTracker : MonoBehaviour
{
    private static List<NpcActor> npcCloseEnoughToInterect = new List<NpcActor>();
    private static HashSet<NpcActor> allNpcs = new HashSet<NpcActor>();
    
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
    
    public static NpcActor GetNpcActorInstance(NpcActor actorToFind)
    {
        foreach (NpcActor actor in allNpcs)
        {
            if (actor.Name == actorToFind.Name) { return actor; }
        }
        return null;
    }
    
    public static NpcActor GetNpcActorInstance(string actorName)
    {
        foreach (NpcActor actor in allNpcs)
        {
            if (actor.Name == actorName) { return actor; }
        }
        return null;
    }

    private void trackNPCsOnSceneLoad()
    {
        NpcActor[] actorsInScene = FindObjectsOfType<NpcActor>();
        foreach (NpcActor actor in actorsInScene)
        {
            allNpcs.Add(actor);
        }
    }
    
    private void OnEnable()
    {
        SceneSwitchManager.onSceneLoaded += trackNPCsOnSceneLoad;
    }

    private void OnDisable()
    {
        SceneSwitchManager.onSceneLoaded -= trackNPCsOnSceneLoad;
    }
}
