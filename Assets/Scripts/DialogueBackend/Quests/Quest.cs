using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    
    [Header("Quest Identifiers")]
    public string questId; //Needed for the backend
    public string questName; // This is for the front-end if needed
    public string questGiverName;

    [TextArea(3, 5)] public string questDescription; //THis is for the front-end if needed
    
    [Header("Quest Dialogues")]
    public Dialogue questInProgressDialogue;
    public Dialogue questCompleteDialogue;
    
    [Header("Quest Settings")]
    public QuestType questType;
    
    [ShowIf("questType", QuestType.CollectItems)] public ItemData itemData;
    [ShowIf("questType", QuestType.CollectItems)] public int itemCount;
    
    [ShowIf("questType", QuestType.KillEnemies)] public string enemyName;
    [ShowIf("questType", QuestType.KillEnemies)] public int requiredKills;
    
    [ShowIf("questType", QuestType.TalkToNpc)] public string npcName;

    [ShowIf("questType", QuestType.ReachLocation)] public string locationName;
    
    public enum QuestType //Add more quest types in here if needed!
    {
        CollectItems,
        KillEnemies,
        TalkToNpc,
        ReachLocation,
        Other,
    }
}

