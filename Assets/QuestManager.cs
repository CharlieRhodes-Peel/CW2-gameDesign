using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    //In the form <activeQuest, currentProgress>
    private Dictionary<Quest, int> activeQuests = new Dictionary<Quest, int>();
    private Dictionary<Quest, int> allQuests = new Dictionary<Quest, int>();
    private HashSet<Quest> completedQuests = new HashSet<Quest>(); // This is for quests that are completed but not FINISHED

    //Maps quests to npc actors names
    private static Dictionary<Quest, Dialogue> currentQuestDialogue = new Dictionary<Quest, Dialogue>();

    public static event Action<ItemData> OnItemGivenAway;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadAllQuests();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAllQuests() //Loads all quests the player could encounter incase they complete them early!
    {
        Quest[] quests = Resources.LoadAll<Quest>("Quests");
        foreach (Quest quest in quests)
        {
            allQuests.Add(quest, 0);
            currentQuestDialogue.Add(quest, quest.questInProgressDialogue);
        }
        
        Debug.Log($"Loaded {allQuests.Count} quests in total!");
    }
    
    //Called when an item is picked up, see if it can increment ANY quest
    private void ItemQuestProgress(ItemData itemData)
    {
        Debug.Log($"Item was picked up {itemData.itemName}");
        List<Quest> quests = new List<Quest>(allQuests.Keys);
        foreach (Quest quest in quests)
        {
            if (quest.questType != Quest.QuestType.CollectItems) { continue; } //Only worry about collect item quests

            if (quest.itemData == itemData)
            {
                allQuests[quest]++; //Incremement Quest progress
                CheckIfQuestComplete(allQuests, quest); //Check all quests against this one
            }
        }
    }

    //Should be called anytime an enemy is killed
    private void KillEnemyProgress(string enemyName)
    {
        //Should only increment ACTIVE quests, unlike the item quests
        List<Quest> quests =  new List<Quest>(this.activeQuests.Keys);
        foreach (Quest quest in quests)
        {
            if (quest.questType != Quest.QuestType.KillEnemies) { continue; } //Only worry about kill enemy quests

            if (quest.enemyName == enemyName)
            {
                activeQuests[quest]++;
                CheckIfQuestComplete(activeQuests, quest);
            }
        }
    }
    
    //Should be called anytime an enemy is killed
    private void KillEnemyPassiveProgress(string enemyName)
    {
        //Should increment all quests
        List<Quest> quests =  new List<Quest>(this.allQuests.Keys);
        foreach (Quest quest in quests)
        {
            if (quest.questType != Quest.QuestType.KillEnemiesPassive) { continue; } //Only worry about kill enemy quests

            if (quest.enemyName == enemyName)
            {
                allQuests[quest]++;
                CheckIfQuestComplete(allQuests, quest);
            }
        }
    }


    private void TalkToNPCProgress(string npcName)
    {
        List<Quest> quests = new List<Quest>(this.allQuests.Keys); //Checks all quests to see if you have spoken to a certain npc

        foreach (Quest quest in quests)
        {
            if (quest.questType != Quest.QuestType.TalkToNpc) { continue; } //Only worry about talk to quests

            if (quest.npcName == npcName)
            {
                activeQuests[quest]++;
                CheckIfQuestComplete(activeQuests, quest);
            }
        }
    }

    private void ReachLocationProgress(string locationName)
    {
        List<Quest> quests = new List<Quest>(this.allQuests.Keys); //Checks all quests to see if you have reached that location

        foreach (Quest quest in quests)
        {
            if (quest.questType != Quest.QuestType.ReachLocation) { continue; } //Only worry about reach location quests

            if (quest.locationName.ToLower() == locationName)
            {
                activeQuests[quest]++;
                CheckIfQuestComplete(activeQuests, quest);
            }
        }
    }

    private void CheckIfQuestComplete(Dictionary<Quest, int> dictionary, Quest quest)
    {
        bool questComplete = false;
        if (!dictionary.ContainsKey(quest)) { return; }

        Quest.QuestType questType = quest.questType;
        switch (questType)
        {
            case Quest.QuestType.CollectItems:
                questComplete = dictionary[quest] >= quest.itemCount; break;
            case Quest.QuestType.KillEnemies: 
                questComplete = dictionary[quest] >= quest.requiredKills; break;
            case Quest.QuestType.KillEnemiesPassive: 
                questComplete = dictionary[quest] >= quest.requiredKills; break;
            case Quest.QuestType.TalkToNpc:
                questComplete = dictionary[quest] >= 1; break;
            case Quest.QuestType.ReachLocation:
                questComplete = dictionary[quest] >= 1; break;
        }

        if (questComplete)
        { QuestComplete(quest); }
    }

    public void StartQuest(Quest quest)
    {
        activeQuests.Add(quest, 0);
        currentQuestDialogue[quest] = quest.questInProgressDialogue;
        
        if (quest.questShownInMenu) {MenuUI.Instance.AddQuestToUI(quest);}
        
        CheckIfQuestComplete(allQuests, quest); //Checks if this quest has already been done!
    }
    public void QuestComplete(Quest quest)
    {
        Debug.Log($"Ending quest with {quest.questGiverName}");
        currentQuestDialogue[quest] = quest.questCompleteDialogue;
        completedQuests.Add(quest);
        
        if (quest.questShownInMenu) {MenuUI.Instance.RemoveQuestFromUI(quest);}
    }

    public void FinishQuest(Quest quest)
    {
        completedQuests.Remove(quest);
        currentQuestDialogue.Remove(quest);
        activeQuests.Remove(quest);
        Quest.QuestType questType = quest.questType;
        switch (questType)
        {
            case Quest.QuestType.CollectItems:
                OnItemGivenAway?.Invoke(quest.itemData);
                break;
            
            case Quest.QuestType.KillEnemies:
                break;
            case Quest.QuestType.TalkToNpc:
                break;
            case Quest.QuestType.ReachLocation:
                break;
        }
    }

    public Dialogue GetActiveDialogue(string name)
    {
        foreach (Quest quest in allQuests.Keys)
        {
            if (quest.questGiverName == name)
            {
                return currentQuestDialogue[quest];
            }
        }
        return null;
    }

    public bool HasQuestWithNPC(string npcName)
    {
        foreach (Quest quest in activeQuests.Keys)
        {
            if (quest.questGiverName == npcName) { return true; }
        }
        return false;
    }

    public int GetActiveQuestProgress(Quest quest)
    {
        if (!activeQuests.ContainsKey(quest)) { return 0; }
        
        return activeQuests[quest];
    }

    public int GetAllQuestProgress(Quest quest)
    {
        if (!allQuests.ContainsKey(quest)) { return 0; }
        return allQuests[quest];
    }
    
    //Subscribing to events that might affect quest objectives
    private void OnEnable()
    {
        Item.OnItemPicked += ItemQuestProgress;
        Enemy.OnEnemyDeathEvent += KillEnemyProgress;
        Enemy.OnEnemyDeathEvent += KillEnemyPassiveProgress;
        NpcActor.OnPlayerInteractEvent += TalkToNPCProgress;
        LocationManager.OnLocationVisited += ReachLocationProgress;
    }
    
    private void OnDisable()
    {
        Item.OnItemPicked -= ItemQuestProgress;
        Enemy.OnEnemyDeathEvent -= KillEnemyProgress;
        Enemy.OnEnemyDeathEvent -= KillEnemyPassiveProgress;
        NpcActor.OnPlayerInteractEvent -= TalkToNPCProgress;
        LocationManager.OnLocationVisited -= ReachLocationProgress;
    }
}
