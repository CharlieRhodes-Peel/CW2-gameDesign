//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
//Dialogue
using UnityEngine;
 
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Asset")]
public class Dialogue : ScriptableObject
{
    //First node of the conversation
    public DialogueNode RootNode;

    public bool conditional = true;
}