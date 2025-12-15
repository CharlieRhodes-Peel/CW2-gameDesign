using System;
using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public string itemName;
    [SerializeField] public string itemID;
    
    
    //Private references
    private SpriteRenderer spriteRenderer;
    
    //Events
    public static event Action<Item> OnItemPicked; //Passes itself at the reference
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SceneSwitchManager.onSceneLoaded += PlayerHasItemCheck;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the player enters out range then pick up
        if (other.CompareTag("Player")) { Pickup(); }
    }

    private void Pickup()
    {
        OnItemPicked?.Invoke(this);
        Destroy(gameObject);
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }

    private void PlayerHasItemCheck()
    {
        GameObject player = FindPlayer();
        
        Debug.Log("I found player he is called!: " + player.name);

        bool playerHasItem = player.GetComponent<PlayerInventory>().isInInventory(itemID);

        if (playerHasItem)
        {
            Destroy(gameObject);
        }
    }

    private GameObject FindPlayer()
    {
        return GameObject.FindWithTag("Player");
    }

    private void OnDestroy()
    {
        SceneSwitchManager.onSceneLoaded -= PlayerHasItemCheck;
    }
}
