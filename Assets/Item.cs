using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public static event Action<ItemData> OnItemPicked;

    private void Start()
    {
        SceneSwitchManager.onSceneLoaded += AlreadyCollectedCheck;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        OnItemPicked?.Invoke(itemData);
        Destroy(gameObject);
    }

    private void AlreadyCollectedCheck()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        bool alreadyCollected = player.GetComponent<PlayerInventory>().HavePickedUpBefore(itemData);

        if (alreadyCollected)
        { Destroy(gameObject); }
    }

    private void OnDestroy()
    {
        //Unsubscribe from all events
        SceneSwitchManager.onSceneLoaded -= AlreadyCollectedCheck;
    }
}
