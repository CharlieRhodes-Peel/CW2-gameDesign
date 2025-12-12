using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    List<Item> items = new List<Item>();
    
    public static event Action<Item> OnItemRemovedFromInventory;

    //Called anytime an item is picked up, passing through the item that wants to be picked up!
    private void OnItemPicked(Item item)
    {
        items.Add(item);
        
        Debug.Log(item.itemName + " was picked up");
    }

    public void RemoveItemFromInventory(Item item)
    {
        items.Remove(item);
        OnItemRemovedFromInventory?.Invoke(item);
    }
    
    
    //Event stuff
    private void OnEnable()
    {
        Item.OnItemPicked += OnItemPicked;
    }

    private void OnDisable()
    {
        Item.OnItemPicked -= OnItemPicked;
    }
}
