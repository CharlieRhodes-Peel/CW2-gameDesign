using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Toggles")]
    [SerializeField] private bool spawnsMoney = true;
    [SerializeField] private bool spawnsItem = false;
    
    [ShowIf("spawnsMoney")] [SerializeField] private int moneyToSpawn;
    [ShowIf("spawnsItem")] [SerializeField] private GameObject itemToSpawn;
    
    [Header("Interact Settings")]
    [SerializeField] private string popupText;
    [SerializeField] private Transform popupPos;
    
    [Header("References")]
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private GameObject openParticles;
    [SerializeField] private SpriteRenderer chestSprite;
    [ShowIf("spawnsMoney")] [SerializeField] private GameObject moneySpawnerPrefab;
    [ShowIf("spawnsItem")] [SerializeField] private Transform itemSpawnPos;
    
    private bool opened;
    private string uniqueID;

    private void Start()
    {
        opened = false;
        
        //Should check if chest has already been opened
        uniqueID = $"{SceneManager.GetActiveScene().name}_{gameObject.name}_{moneyToSpawn}";
        if (ChestStateManager.openedChests.Contains(uniqueID))
        {
            opened = true;
            chestSprite.sprite = openedSprite;
        }
    }
    
    public void Interact()
    {
        if (spawnsMoney) {SpawnMoney();}
        if  (spawnsItem) {SpawnItem();}
     
        //Declare opened
        opened = true;
        InteractManager.UnregisterInteractable(this);
        ChestStateManager.ChestOpened(uniqueID);
        
        //Visually show opened
        chestSprite.sprite = openedSprite;
        Instantiate(openParticles, transform.position, Quaternion.identity);
    }

    private void SpawnMoney()
    {
        GameObject moneySpawnerObject = Instantiate(moneySpawnerPrefab, transform.position, Quaternion.identity);
        MoneySpawner moneySpawner = moneySpawnerObject.GetComponent<MoneySpawner>();
        
        moneySpawner.moneyToSpawn = moneyToSpawn;
        moneySpawner.Spawn();
    }

    private void SpawnItem()
    {
        Instantiate(itemToSpawn, itemSpawnPos.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) {return;}
        if (other.CompareTag("Player"))
        {
            InteractManager.RegisterInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if  (opened) {return;}
        if (other.CompareTag("Player"))
        {
            InteractManager.UnregisterInteractable(this);
        }
    }    
        
    public Vector3 GetInteractPopupPosition()
    {
        return popupPos.position;
    }

    public string GetInteractPopupText()
    {
        return popupText;
    }
}
