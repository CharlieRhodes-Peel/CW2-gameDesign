using System;
using UnityEngine;

public class Money: MonoBehaviour
{
    public static event Action<int> OnMoneyPickup;

    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) { return; }
        
        OnMoneyPickup?.Invoke(1);
        
        Destroy(gameObject);
    }
}