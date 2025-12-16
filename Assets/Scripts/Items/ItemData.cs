using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Item Identifiers")]
    public string itemName;
    public Sprite itemUIIcon;
    public ItemType itemType;
    
    public enum ItemType
    {
        QuestObjective,
        Other
    }
}
