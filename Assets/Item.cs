using System;
using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public string itemName;
    
    //Private references
    private SpriteRenderer spriteRenderer;
    
    //Events
    public static event Action<Item> OnItemPicked; //Passes itself at the reference
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
}
