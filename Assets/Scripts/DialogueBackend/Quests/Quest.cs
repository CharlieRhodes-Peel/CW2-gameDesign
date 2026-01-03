using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]

//Quests HAVE to be placed Resources/Quests to be picked up by the Quest manager

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
    public bool questShownInMenu = true;
    
    [ShowIf("questType", QuestType.CollectItems)] public ItemData itemData;
    [ShowIf("questType", QuestType.CollectItems)] public int itemCount;
    
    [ShowIf(EConditionOperator.Or, "IsKillEnemiesQuest", "IsKillEnemiesPassiveQuest")] public string enemyName;
    [ShowIf(EConditionOperator.Or, "IsKillEnemiesQuest", "IsKillEnemiesPassiveQuest")]  public int requiredKills;
    
    [ShowIf("questType", QuestType.TalkToNpc)] public string npcName;

    [ShowIf("questType", QuestType.ReachLocation)] public string locationName;
    
    public enum QuestType //Add more quest types in here if needed!
    {
        CollectItems,
        KillEnemies,
        KillEnemiesPassive,
        TalkToNpc,
        ReachLocation,
        Other,
    }
    
    // Helper methods for ShowIf conditions
    private bool IsKillEnemiesQuest() => questType == QuestType.KillEnemies;
    private bool IsKillEnemiesPassiveQuest() => questType == QuestType.KillEnemiesPassive;
}

