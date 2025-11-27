//Dialogue Node
using System;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class DialogueNode
{
    public string dialogueText;
    public List<DialogueResponse> responses;
    
    public UnityEvent onDialogueStart;
 
    internal bool IsLastNode()
    {
        return responses.Count <= 0;
    }
}