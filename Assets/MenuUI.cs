using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public static MenuUI Instance;

    [Header("Inventory Stuff")]
    [SerializeField] private GameObject inventoryGrid;
    [SerializeField] private GameObject itemHolderUIPrefab;
    [SerializeField] private GameObject itemDescptionUI;
    
    [Header("Quest Stuff")]
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private GameObject questGrid;
    [SerializeField] private GameObject questHolderUIPrefab;
    [SerializeField] private GameObject questDescriptionUI;
    [SerializeField] private List<Sprite> questIcons;
    
    [Header("Shop Stuff")]
    [SerializeField] private TextMeshProUGUI shopTitle;
    [SerializeField] private GameObject shopGrid;
    [SerializeField] private GameObject shopGridVisual;
    
    [Header("Generals")]
    [SerializeField] private InputActionReference openInventoryAction;
    [SerializeField] private InputActionReference closeInventoryAction;
    [SerializeField] private ToolTip toolTip;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationDelay;
    
    [SerializeField] private PlayerInventory playerInventory;
    
    
    //Private dictionaries
    private Dictionary<ItemData, ItemUI> inventoryItems = new Dictionary<ItemData, ItemUI>(); //Bridges ItemData to ItemUI for inventory items
    private Dictionary<ItemData, ItemUI> shopItems = new  Dictionary<ItemData, ItemUI>(); //Bridges ItemData to ItemUI for shop items
    private Dictionary<Quest, QuestUI> questsToUI = new Dictionary<Quest, QuestUI>(); //Bridges Quest to QuestUI
    private Dictionary<QuestUI, Quest> UIToquests = new  Dictionary<QuestUI, Quest>();
    
    //Logic flag
    private GameObject latestUIElement; //To keep track of the latest element to be added
    private bool isMenuOpen = false;
    public static bool shopShowing = false;
    public static bool questsShowing = true;
    
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
        openInventoryAction.action.performed += OnInventoryOpen;
        closeInventoryAction.action.performed += OnMenuClose;

        PlayerInventory.OnItemPickedUp += AddItemToInventoryUI;
        PlayerInventory.OnItemRemoved += RemoveItemFromInventoryUI;
    }

    //Called when an item is picked up and said item is passed through
    private void AddItemToInventoryUI(ItemData itemData)
    {
        //Check if item already exists
        if (inventoryItems.ContainsKey(itemData))
        {
            ItemUI existingUI = inventoryItems[itemData];
            existingUI.IncrementQuantity();

            latestUIElement = existingUI.gameObject;
            MenuPopupUI.Instance.ShowPopup(itemData);
            return;
        }
        
        //Create the item holder
        GameObject newItemHolder = Instantiate(itemHolderUIPrefab, inventoryGrid.transform);
        
        ItemUI itemUI = newItemHolder.GetComponent<ItemUI>();

        itemUI.itemIcon.sprite = itemData.itemUIIcon;
        itemUI.itemName.text = itemData.itemName;
        itemUI.description = itemData.description;
        itemUI.value = itemData.value;
        itemUI.canBuy = false;
        itemUI.quantity = 1;
        
        inventoryItems.Add(itemData, itemUI);
        
        latestUIElement = newItemHolder;
        
        MenuPopupUI.Instance.ShowPopup(itemData);
    }

    public void AddItemToShopUI(ItemData itemData)
    {
        //Create the item holder
        GameObject newItemHolder = Instantiate(itemHolderUIPrefab, shopGrid.transform);
        
        ItemUI itemUI = newItemHolder.GetComponent<ItemUI>();

        itemUI.itemIcon.sprite = itemData.itemUIIcon;
        itemUI.itemName.text = itemData.itemName;
        itemUI.description = itemData.description;
        itemUI.value = itemData.value;
        itemUI.canBuy = true;
        itemUI.quantity = 1;
        
        shopItems.Add(itemData, itemUI);
        
        latestUIElement = newItemHolder;
    }
        
    //Called anytime the player removes an item from their inventory
    public void RemoveItemFromInventoryUI(ItemData itemData)
    {
        if (!inventoryItems.ContainsKey(itemData)) { return; }
        
        ItemUI existingUI = inventoryItems[itemData];

        //If there are multiple just decrement
        if (existingUI.quantity > 1)
        {
            existingUI.DecrementQuantity();
            return;
        }
        
        inventoryItems[itemData].RemoveUI();
        inventoryItems.Remove(itemData);
    }

    public void RemoveItemFromShopUI(ItemData itemData)
    {
        if (!shopItems.ContainsKey(itemData)) { return; }
        
        shopItems[itemData].RemoveUI();
        shopItems.Remove(itemData);
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
        questUI.questIcon.sprite = questIcons[(int)quest.questType];
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
    private void OnInventoryOpen(InputAction.CallbackContext ctx)
    {
        questsShowing = true;
        shopShowing = false;
        OpenMenu();
    }

    public void OpenShop()
    {
        questsShowing = false;
        shopShowing = true;
        OpenMenu();
    }

    private void OpenMenu()
    {
        isMenuOpen = true;
        
        ActivateMenuItems();

        playerInput.SwitchCurrentActionMap("UI");

        animator.SetTrigger("Open");
        SelectDefaultButton();
    }

    private void ActivateMenuItems()
    {
        gameObject.SetActive(true);

        if (questsShowing)
        {
            questTitle.gameObject.SetActive(true);
            questGrid.gameObject.SetActive(true);
        }
        else
        {
            questTitle.gameObject.SetActive(false);
            questGrid.gameObject.SetActive(false);
        }
        if (shopShowing)
        {
            shopTitle.gameObject.SetActive(true);
            shopGrid.gameObject.SetActive(true);
            shopGridVisual.SetActive(true);
        }
        else
        {
            shopTitle.gameObject.SetActive(false);
            shopGrid.gameObject.SetActive(false);
            shopGridVisual.SetActive(false);
        }
        
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
        if (latestUIElement == null) { return; }
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

    public void Buy(ItemUI itemUI)
    {
        foreach (ItemData itemData in shopItems.Keys)
        {
            if (itemData.itemName == itemUI.itemName.text)
            {
                if (itemData.value > playerInventory.currency) { return;}
                
                playerInventory.MoneyPickedUp(-itemData.value);
                playerInventory.ItemPickedUp(itemData);
                
                ItemDescriptorUI.Hide();
                RemoveItemFromShopUI(itemData);
            }
        }
    }
    
    private void OnDestroy()
    {
        //Unsubscribe from all events
        PlayerInventory.OnItemRemoved -= RemoveItemFromInventoryUI;
        PlayerInventory.OnItemPickedUp -= AddItemToInventoryUI;
        openInventoryAction.action.performed -= OnInventoryOpen;
    }
}
