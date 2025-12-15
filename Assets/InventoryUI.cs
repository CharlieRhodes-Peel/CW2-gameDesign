using System;
using UnityEditor.Timeline.Actions;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI instance;
    
    [SerializeField] private GameObject itemHolderUIPrefab;
    
    [SerializeField] private InputActionReference inventoryAction;

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
        
        SceneSwitchManager.onSceneLoaded += SubscribeToItemsInRoom;
        SceneSwitchManager.onSceneExit += UnsubscribeFromItemsInRoom;
    }

    //Called when an item is picked up and said item is passed through
    private void AddItemToUI(Item item)
    {
        //Create the item holder
        GameObject newItemHolder = Instantiate(itemHolderUIPrefab, transform);

        //This is because it needs to find the correct image, bit jank I know, but it works (sue me!)
        Image[] images = newItemHolder.GetComponentsInChildren<Image>();
        images[1].sprite = item.GetSprite();
    }
    
        
    //Called anytime the player presses the inventory button
    private void OnInventoryPressed(InputAction.CallbackContext ctx)
    {
        if (isInventoryOpen) { gameObject.SetActive(false); }
        else                 { gameObject.SetActive(true); } 
        
        isInventoryOpen = !isInventoryOpen;
    }

    //Allowing this to be called from somewhere
    public void RemoveItemFromUI(Item item)
    {
        GameObject itemHolder = FindItemGameObject(item);
        if (itemHolder == null) { return; }
        
        Destroy(itemHolder);
    }

    private GameObject FindItemGameObject(Item item)
    {
        GameObject[] items = item.GetComponentsInChildren<GameObject>();

        foreach (GameObject i in items) {
            if (GetItemImage(i).sprite == item.GetSprite())
            {
                return i;
            }
        }
        return null;
    }

    private Image GetItemImage(GameObject itemHolder)
    {
        //This is because it needs to find the correct image, bit jank I know, but it works (sue me!)
        Image[] images = itemHolder.GetComponentsInChildren<Image>();
        return images[1]; 
    }

    private void SubscribeToItemsInRoom()
    {
        Item.OnItemPicked += AddItemToUI;
    }

    private void UnsubscribeFromItemsInRoom()
    {
        Item.OnItemPicked -= AddItemToUI;
    }

    private void OnDestroy()
    {
        //Unsubscribe from all events
        Item.OnItemPicked -= AddItemToUI;
        SceneSwitchManager.onSceneLoaded -= SubscribeToItemsInRoom;
        SceneSwitchManager.onSceneExit -= UnsubscribeFromItemsInRoom;
        inventoryAction.action.performed -= OnInventoryPressed;
    }
}
