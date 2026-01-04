using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Item Identifiers")]
    public ItemType itemType;
    public string itemName;
    public Sprite itemUIIcon;
    
    [TextArea(3,5)]
    public string description;
    
    [ShowIf("itemType", ItemType.Power)] public PlayerMovement.AbilityUnlocks abilityUnlocks = PlayerMovement.AbilityUnlocks.None;
    
    public enum ItemType
    {
        QuestObjective,
        Power,
        Other
    }
}
