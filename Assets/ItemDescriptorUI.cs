using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDescriptorUI : MonoBehaviour
{
    public static ItemDescriptorUI instance;
    
    public PlayerInventory player;

    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image icon;
    public TextMeshProUGUI price;

    public GameObject buyButton;
    private ItemUI currentItemShown;
    
    public GameObject backbutton;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {Destroy(gameObject);}
    }

    private void Start()
    {
        instance.gameObject.SetActive(false);
    }

    public void Set(ItemUI itemUI)
    {
        currentItemShown = itemUI;
        title.text = itemUI.itemName.text;
        description.text = itemUI.description;
        icon.sprite = itemUI.itemIcon.sprite;
        price.text = itemUI.value.ToString();
    }

    public void Show(ItemUI itemUI)
    {
        Set(itemUI);
        instance.gameObject.SetActive(true);

        if (MenuUI.shopShowing && itemUI.canBuy)
        {
            buyButton.SetActive(true);
            EventSystem.current.SetSelectedGameObject(instance.buyButton);
        }
        else
        {
            buyButton.SetActive(false);
            EventSystem.current.SetSelectedGameObject(instance.backbutton);
        }
    }

    public static void Hide()
    {
        instance.gameObject.SetActive(false);
        
        MenuUI.Instance.SelectDefaultButton();
    }

    public void Buy()
    {
        MenuUI.Instance.Buy(currentItemShown);
    }
}
