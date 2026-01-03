using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public static MenuUI Instance;

    [SerializeField] private GameObject inventoryGrid;
    [SerializeField] private GameObject questGrid;
    [SerializeField] private GameObject itemHolderUIPrefab;
    [SerializeField] private GameObject questHolderUIPrefab;
    [SerializeField] private InputActionReference openInventoryAction;
    [SerializeField] private InputActionReference closeInventoryAction;
    [SerializeField] private GameObject itemDescptionUI;
    [SerializeField] private GameObject questDescriptionUI;
    
    [SerializeField] private ToolTip toolTip;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationDelay;
    
    private Dictionary<ItemData, ItemUI> items = new Dictionary<ItemData, ItemUI>(); //Bridges ItemData to ItemUI
    private Dictionary<Quest, QuestUI> questsToUI = new Dictionary<Quest, QuestUI>(); //Bridges Quest to QuestUI
    private Dictionary<QuestUI, Quest> UIToquests = new  Dictionary<QuestUI, Quest>();

    [SerializeField] private List<Sprite> questIcons;
    
    private GameObject latestUIElement; //To keep track of the latest element to be added

    //Logic flag
    private bool isMenuOpen = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
    }
    
    private void Start()
    {
        gameObject.SetActive(false);
        
        //Events
        openInventoryAction.action.performed += OnMenuOpen;
        closeInventoryAction.action.performed += OnMenuClose;

        PlayerInventory.OnItemPickedUp += AddItemToUI;
        PlayerInventory.OnItemRemoved += RemoveItemFromUI;
    }

    //Called when an item is picked up and said item is passed through
    private void AddItemToUI(ItemData itemData)
    {
        //Create the item holder
        GameObject newItemHolder = Instantiate(itemHolderUIPrefab, inventoryGrid.transform);
        
        ItemUI itemUI = newItemHolder.GetComponent<ItemUI>();

        itemUI.itemIcon.sprite = itemData.itemUIIcon;
        itemUI.itemName.text = itemData.itemName;
        itemUI.description = itemData.description;
        
        items.Add(itemData, itemUI);
        
        latestUIElement = newItemHolder;
        
        MenuPopupUI.Instance.ShowPopup(itemData);
    }
        
    //Called anytime the player removes an item from their inventory
    public void RemoveItemFromUI(ItemData itemData)
    {
        if (!items.ContainsKey(itemData)) { return; }
        
        items[itemData].RemoveUI();
        items.Remove(itemData);
    }

    public void AddQuestToUI(Quest quest)
    {
        GameObject newQuestHolder = Instantiate(questHolderUIPrefab, questGrid.transform);
        
        QuestUI questUI = newQuestHolder.GetComponent<QuestUI>();

        questUI = QuestToUIBridge(quest, questUI);

        questsToUI.Add(quest, questUI);
        UIToquests.Add(questUI, quest);
        
        latestUIElement = newQuestHolder;
        
        MenuPopupUI.Instance.ShowPopup(quest, true);
    }

    public void RemoveQuestFromUI(Quest quest)
    {
        if(!questsToUI.ContainsKey(quest)) { return; }
        questsToUI[quest].RemoveUI();
        
        UIToquests.Remove(questsToUI[quest]);
        questsToUI.Remove(quest);
        
        MenuPopupUI.Instance.ShowPopup(quest, false);
    }
    
    private QuestUI QuestToUIBridge(Quest quest, QuestUI questUI)
    {
        //Settings things
        questUI.questIcon.sprite = questIcons[(int)questUI.questType];
        questUI.questName.text = quest.questName;
        questUI.description = quest.questDescription;
        questUI.questType = quest.questType;
        questUI.questGiver = quest.questGiverName;

        if (quest.questType == Quest.QuestType.KillEnemies || quest.questType == Quest.QuestType.KillEnemiesPassive)
        {
            questUI.amountNeeded = quest.requiredKills;
            questUI.currentProgress = QuestManager.Instance.GetActiveQuestProgress(quest);
        }
        
        else if (quest.questType == Quest.QuestType.CollectItems)
        {
            questUI.amountNeeded = quest.itemCount;
            questUI.currentProgress = QuestManager.Instance.GetActiveQuestProgress(quest);
        }

        return questUI;
    }
    
    //Called anytime the player presses the inventory button while using the "Player" action map
    private void OnMenuOpen(InputAction.CallbackContext ctx)
    {
        isMenuOpen = true;
        
        gameObject.SetActive(true);
        playerInput.SwitchCurrentActionMap("UI");

        animator.SetTrigger("Open");
        SelectDefaultButton();
    }

    //Called anytime the player presses the inventory button while using the "UI" action map
    private void OnMenuClose(InputAction.CallbackContext ctx)
    {
        if (itemDescptionUI.activeInHierarchy || questDescriptionUI.activeInHierarchy)
        {
            itemDescptionUI.SetActive(false);
            questDescriptionUI.SetActive(false);
            EventSystem.current.SetSelectedGameObject(latestUIElement);
            return;
        }

        StartCoroutine(DoCloseAnimation());
    }
    
    public void SelectDefaultButton()
    {
        GameObject buttonToSelect = latestUIElement;
        if (latestUIElement == null) {return; }
        EventSystem.current.SetSelectedGameObject(latestUIElement);
    }

    private IEnumerator DoCloseAnimation()
    {
        animator.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(animationDelay);
        
        isMenuOpen = false;
        gameObject.SetActive(false);
        
        ToolTipSystem.instance.tooltip.gameObject.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");
    }

    public int QueryQuestProgress(QuestUI questUI)
    {
        Quest quest = UIToquests[questUI];
        
        if (quest.questType == Quest.QuestType.CollectItems || quest.questType == Quest.QuestType.KillEnemiesPassive)
        {
            return QuestManager.Instance.GetAllQuestProgress(quest);
        }
        if (quest.questType == Quest.QuestType.KillEnemies)
        {
            return QuestManager.Instance.GetActiveQuestProgress(quest);
        }

        return 0;

    }
    
    private void OnDestroy()
    {
        //Unsubscribe from all events
        PlayerInventory.OnItemRemoved -= RemoveItemFromUI;
        PlayerInventory.OnItemPickedUp -= AddItemToUI;
        openInventoryAction.action.performed -= OnMenuOpen;
    }
}
