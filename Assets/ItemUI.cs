using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public Image itemIcon;
    [SerializeField] public TextMeshProUGUI itemName;
    [HideInInspector] public string description;
    [HideInInspector] public int value;
    [HideInInspector] public bool canBuy;

    public void RemoveUI()
    {
        Destroy(gameObject);
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
