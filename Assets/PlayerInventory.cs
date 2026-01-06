using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    
    public List<ItemData> itemsPickedUp = new List<ItemData>(); //This should only be ADDED to

    public int currency = 0;
    
    [SerializeField] private TextMeshProUGUI currencyUI;
    [SerializeField] private AudioClip itemPickedSound;
    
    private PlayerMovement playerMovement;

    public static event Action<ItemData> OnItemRemoved;
    public static event Action<ItemData> OnItemPickedUp;

    private void Start()
    {
        currencyUI.text = currency.ToString();
        playerMovement = GetComponent<PlayerMovement>();
        
    }

    public void ItemPickedUp(ItemData item)
    {
        items.Add(item);
        itemsPickedUp.Add(item);
        
        //Sound
        SoundManager.Instance.PlaySoundEffect(itemPickedSound, transform, 1);
        
        OnItemPickedUp?.Invoke(item); //This is to tell the Inventory UI

        //If this item doesn't do anything then skip 
        if (item.abilityUnlocks != PlayerMovement.AbilityUnlocks.None)
        {
            UnlockAbility(item.abilityUnlocks); 
        }

        if (item.itemType == ItemData.ItemType.DamageIncrease)
        {
            IncreaseDamage(item.increaseDamageBy);
        }
    }
    
    private void ItemRemoved(ItemData item)
    {
        items.Remove(item);
        OnItemRemoved?.Invoke(item);
    }

    public void MoneyPickedUp(int amount)
    {
        currency+= amount;
        currencyUI.text = currency.ToString();
    }
    
    
    //Event stuff
    private void OnEnable()
    {
        Item.OnItemPicked += ItemPickedUp;
        QuestManager.OnItemGivenAway += ItemRemoved;
        Money.OnMoneyPickup += MoneyPickedUp;
    }

    private void OnDisable()
    {
        Item.OnItemPicked -= ItemPickedUp;
        QuestManager.OnItemGivenAway -= ItemRemoved;
        Money.OnMoneyPickup -= MoneyPickedUp;
    }

    public bool HavePickedUpBefore(ItemData itemData)
    {
        return itemsPickedUp.Contains(itemData);
    }

    private void UnlockAbility(PlayerMovement.AbilityUnlocks abilityUnlock)
    {
        switch (abilityUnlock)
        {
            case PlayerMovement.AbilityUnlocks.Dash:
                playerMovement.dashUnlocked = true; break;
            case PlayerMovement.AbilityUnlocks.DoubleJump:
                playerMovement.doubleJumpUnlocked = true; break;
            case PlayerMovement.AbilityUnlocks.WallClimbing:
                playerMovement.wallClimbingUnlocked = true; break;
        }
    }

    private void IncreaseDamage(float damageIncrease)
    {
        PlayerAttack pAttack = GetComponent<PlayerAttack>();
        
        pAttack.AddToDamagePerHit(damageIncrease);
    }
}
