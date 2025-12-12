using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    [Header("Basic Info")] public string questId; //Needed for the backend
    public string questName; // This is for the front-end if needed

    [TextArea(3, 5)] public string questDescription; //THis is for the front-end if needed

    public enum QuestType //Add more quest types in here if needed!
    {
        CollectItems,
        KillEnemies,
        TalkToNpc,
        ReachLocation,
        Other,
    }
    
    public QuestType questType;
    
    [Header("Item Collection Quests")]
    public string itemName;
    public int itemCount;

    [Header("Killing Enemies Quests")]
    public string enemyName;
    public int requiredKills;
    
    [Header("Talking to Npc Quests")]
    public string npcName;

    [Header("Reach Location Quests")]
    public string locationName;

    [HideInInspector] public int currentProgress;

    public bool IsCompleted()
    {
        switch (questType)
        {
            case QuestType.CollectItems:
                return currentProgress >= itemCount;
            case QuestType.KillEnemies:
                return currentProgress >= requiredKills;
            case QuestType.TalkToNpc:
                return currentProgress >= 1;
            case QuestType.ReachLocation:
                return currentProgress >= 1;
        }
        return false;
    }

    public void IncrementProgress()
    {
        currentProgress += 1;
    }
    
}

