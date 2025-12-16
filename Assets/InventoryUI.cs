using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    
    [SerializeField] private GameObject itemHolderUIPrefab;
    [SerializeField] private InputActionReference openInventoryAction;
    [SerializeField] private InputActionReference closeInventoryAction;
    
    [SerializeField] private ToolTip toolTip;
    [SerializeField] private PlayerInput playerInput;
    
    private Dictionary<ItemData, ItemUI> items = new Dictionary<ItemData, ItemUI>(); //Bridges ItemData to ItemUI

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
    }
    
    //Logic flag
    private bool isInventoryOpen = false;
    
    private void Start()
    {
        gameObject.SetActive(false);
        
        //Events
        openInventoryAction.action.performed += OnInventoryOpen;
        closeInventoryAction.action.performed += OnInventoryClose;

        PlayerInventory.OnItemPickedUp += AddItemToUI;
        PlayerInventory.OnItemRemoved += RemoveItemFromUI;
    }

    //Called when an item is picked up and said item is passed through
    private void AddItemToUI(ItemData itemData)
    {
        //Create the item holder
        GameObject newItemHolder = Instantiate(itemHolderUIPrefab, transform);
        
        ItemUI itemUI = newItemHolder.GetComponent<ItemUI>();

        itemUI.itemIcon.sprite = itemData.itemUIIcon;
        itemUI.itemName.text = itemData.itemName;
        itemUI.description = itemData.description;
        
        items.Add(itemData, itemUI);
    }
    
    //Called anytime the player presses the inventory button while using the "Player" action map
    private void OnInventoryOpen(InputAction.CallbackContext ctx)
    {
        isInventoryOpen = true;
        
        gameObject.SetActive(true);
        playerInput.SwitchCurrentActionMap("UI");
        
        Time.timeScale = 0;
    }

    //Called anytime the player presses the inventory button while using the "UI" action map
    private void OnInventoryClose(InputAction.CallbackContext ctx)
    {
        isInventoryOpen = false;
        gameObject.SetActive(false);
        
        ToolTipSystem.instance.tooltip.gameObject.SetActive(false);
        playerInput.SwitchCurrentActionMap("Player");

        Time.timeScale = 1;
    }
    
    //Called anytime the player removes an item from their inventory
    public void RemoveItemFromUI(ItemData itemData)
    {
        if (!items.ContainsKey(itemData)) { return; }
        
        items[itemData].RemoveUI();
        items.Remove(itemData);
    }
    
    private void OnDestroy()
    {
        //Unsubscribe from all events
        PlayerInventory.OnItemRemoved -= RemoveItemFromUI;
        PlayerInventory.OnItemPickedUp -= AddItemToUI;
        openInventoryAction.action.performed -= OnInventoryOpen;
    }
}
