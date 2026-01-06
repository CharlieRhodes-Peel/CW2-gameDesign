using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public Image itemIcon;
    [SerializeField] public TextMeshProUGUI itemName;
    [SerializeField] public TextMeshProUGUI quantityText;
    [HideInInspector] public int quantity = 1;
    [HideInInspector] public string description;
    [HideInInspector] public int value;
    [HideInInspector] public bool canBuy;
    
    [SerializeField] public AudioClip[] clickSound;

    public void RemoveUI()
    {
        Destroy(gameObject);
    }

    public void IncrementQuantity()
    {
        quantity += 1;
        quantityText.text = quantity.ToString();
    }

    public void DecrementQuantity()
    {
        quantity -= 1;
        quantityText.text = quantity > 1 ? quantity.ToString() : "";
    }

    public void ClickSound()
    {
        SoundManager.Instance.PlayRandomSoundEffect(clickSound, transform, 1);
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipSystem.Show(itemName.text, description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.Hide();
    }

    public void ItemSelected()
    {
        ItemDescriptorUI.instance.Show(this);
    }
}
