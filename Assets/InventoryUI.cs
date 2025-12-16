using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI instance;
    
    [SerializeField] private GameObject itemHolderUIPrefab;
    
    [SerializeField] private InputActionReference inventoryAction;
    
    private Dictionary<ItemData, GameObject> items = new Dictionary<ItemData, GameObject>(); //Connects ItemData to Icons on inventory

    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        instance = this;
    }
    
    //Logic flag
    private bool isInventoryOpen = false;
    
    private void Start()
    {
        gameObject.SetActive(false);
        
        //Events
        inventoryAction.action.performed += OnInventoryPressed;

        PlayerInventory.OnItemPickedUp += AddItemToUI;
        PlayerInventory.OnItemRemoved += RemoveItemFromUI;
    }
    
    //Called when an item is picked up and said item is passed through
    private void AddItemToUI(ItemData itemData)
    {
        //Create the item holder
        GameObject newItemHolder = Instantiate(itemHolderUIPrefab, transform);
    
        //This is because it needs to find the correct image, bit jank I know, but it works (sue me!)
        Image[] images = newItemHolder.GetComponentsInChildren<Image>();
        images[1].sprite = itemData.itemUIIcon;
        
        items.Add(itemData, newItemHolder);
    }
    
    //Called anytime the player presses the inventory button
    private void OnInventoryPressed(InputAction.CallbackContext ctx)
    {
        if (isInventoryOpen) { gameObject.SetActive(false); }
        else                 { gameObject.SetActive(true); } 
        
        isInventoryOpen = !isInventoryOpen;
    }
    
    //Called anytime the player removes an item from their inventory
    public void RemoveItemFromUI(ItemData itemData)
    {
        if (!items.ContainsKey(itemData)) { return; }
        
        GameObject itemHolder = items[itemData];
        items.Remove(itemData);
        Destroy(itemHolder);
    }
    
    private void OnDestroy()
    {
        //Unsubscribe from all events
        PlayerInventory.OnItemRemoved -= RemoveItemFromUI;
        PlayerInventory.OnItemPickedUp -= AddItemToUI;
        inventoryAction.action.performed -= OnInventoryPressed;
    }
}
