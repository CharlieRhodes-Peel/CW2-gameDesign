//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
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