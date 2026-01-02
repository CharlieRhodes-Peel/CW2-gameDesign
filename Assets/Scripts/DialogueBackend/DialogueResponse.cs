//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
//Dialogue Response

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DialogueResponse
{
    public string responseText;
    [FormerlySerializedAs("friendshipLevelNeeded")] public int friendshipThreshold;
    public DialogueNode nextNode;
    public Dialogue optionalNextDialogueTree;
}