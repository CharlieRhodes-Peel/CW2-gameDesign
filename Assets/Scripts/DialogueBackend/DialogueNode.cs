//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
//Dialogue Node
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueNode
{
    [TextArea(3, 5)]
    public string dialogueText;
    public List<DialogueResponse> responses;

    public UnityEvent onDialogue;
 
    internal bool IsLastNode()
    {
        return responses.Count <= 0;
    }
}