using System;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemData itemData;
    [SerializeField] private Transform popUpPos;
    [SerializeField] private string popUpText;
    [SerializeField] private GameObject pickupParticles;
    
    public static event Action<ItemData> OnItemPicked;
    
    private void Start()
    {
        SceneSwitchManager.onSceneLoaded += AlreadyCollectedCheck;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractManager.RegisterInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractManager.UnregisterInteractable(this);
        }
    }

    private void Pickup()
    {
        Debug.Log("Item picked up");
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
        Instantiate(pickupParticles, transform.position, Quaternion.identity);
        
        //Unsubscribe from all events
        SceneSwitchManager.onSceneLoaded -= AlreadyCollectedCheck;
    }

    //Called when the player interacts with it
    public void Interact()
    {
        Pickup();
    }

    public Vector3 GetInteractPopupPosition()
    {
        return popUpPos.position;
    }

    public string GetInteractPopupText()
    {
        return popUpText;
    }
}
