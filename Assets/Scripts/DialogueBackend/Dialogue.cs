//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
//Dialogue

using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
 
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Root Node")]
public class Dialogue : ScriptableObject
{
    public bool afterBossAction = false;
    [ShowIf("afterBossAction")] public List<BossManager.Bosses> bossesKilled = new List<BossManager.Bosses>();
    [ShowIf("afterBossAction")] public List<BossManager.Bosses> bossesHelped = new List<BossManager.Bosses>();
    //First node of the conversation
    public DialogueNode RootNode;
}