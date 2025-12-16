using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    
    public List<ItemData> itemsPickedUp = new List<ItemData>(); //This should only be ADDED to

    public static event Action<ItemData> OnItemRemoved;
    public static event Action<ItemData> OnItemPickedUp;
    
    private void ItemPickedUp(ItemData item)
    {
        items.Add(item);
        itemsPickedUp.Add(item);
        
        OnItemPickedUp?.Invoke(item); //This is to tell the Inventory UI
    }
    
    private void ItemRemoved(ItemData item)
    {
        items.Remove(item);
        OnItemRemoved?.Invoke(item);
    }
    
    
    //Event stuff
    private void OnEnable()
    {
        Item.OnItemPicked += ItemPickedUp;
        QuestManager.OnItemGivenAway += ItemRemoved;
    }

    private void OnDisable()
    {
        Item.OnItemPicked -= ItemPickedUp;
        QuestManager.OnItemGivenAway -= ItemRemoved;
    }

    public bool HavePickedUpBefore(ItemData itemData)
    {
        return itemsPickedUp.Contains(itemData);
    }
}
