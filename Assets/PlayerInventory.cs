using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<string> items = new List<string>();

    //Called anytime an item is picked up, passing through the item that wants to be picked up!
    private void OnItemPicked(Item item)
    {
        items.Add(item.itemID);
        
        Debug.Log(item.itemName + " was picked up");
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

    public bool isInInventory(string itemID)
    {
        return items.Contains(itemID);
    }
}
