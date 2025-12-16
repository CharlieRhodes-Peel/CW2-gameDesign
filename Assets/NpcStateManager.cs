using System.Collections.Generic;
using UnityEngine;

public class NpcStateManager : MonoBehaviour
{
    public static NpcStateManager Instance;

    //Used to save npc states
    private Dictionary<string, NpcStates.State> npcStates = new Dictionary<string, NpcStates.State>();

    public NpcStates.State GetState(string name)
    {
        if (!npcStates.ContainsKey(name)) { return  NpcStates.State.Neutral; }
        return npcStates[name];
    }

    public void UpdateState(string name,  NpcStates.State state)
    {
        npcStates[name] = state;
    }

    public bool KeepingTrackOf(string name)
    {
        return npcStates.ContainsKey(name);
    }
    
    private void Awake() //Boring ass single instance code
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    
    
}
