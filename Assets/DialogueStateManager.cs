using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStateManager : MonoBehaviour
{
    public static DialogueStateManager instance {get; private set;}
    
    private HashSet<Dialogue> finishedDialogues = new HashSet<Dialogue>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MakeDialogueInActive(Dialogue dialogue)
    {
        finishedDialogues.Add(dialogue);
    }

    public void MakeDialogueActive(Dialogue dialogue)
    {
        finishedDialogues.Remove(dialogue);
    }

    public bool IsDialogueActive(Dialogue dialogue)
    {
        return !finishedDialogues.Contains(dialogue);
    }
}
